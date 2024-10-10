using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;
using SwipetorApp.System.UserRoleAuth;

namespace SwipetorApp.Areas.HostMaster;

[Area(AreaNames.Admin)]
[UserRoleAuth(UserRole.HostMaster)]
public class ImportController(IDbProvider dbProvider, ILogger<ImportController> logger) : Controller
{
    public IActionResult Locations()
    {
        var countriesWithProvinces = new List<string>
            { "US", "CA", "AU", "CN", "MX", "MY", "ES", "IN", "SA", "PK", "BR", "TH", "RU" };

        // countryToCountinent Iso2 both key and value
        var countryToContinent = new Dictionary<string, string>();
        using (var reader = new StreamReader("../_dev/_files/country-continents.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            while (csv.Read()) countryToContinent.Add(csv.GetField<string>(3).Trim(), csv.GetField<string>(0).Trim());
        }

        var countryIso2ToContinentLocation = new Dictionary<string, Location>();

        using var db = dbProvider.Create();

        // Add continents to Locations table
        foreach (var cc in countryToContinent)
        {
            var continent = db.Locations.SingleOrDefault(l => l.Iso2 == cc.Value && l.Type == LocationType.Continent);

            if (continent == null)
            {
                continent = new Location
                {
                    Iso2 = cc.Value,
                    Type = LocationType.Continent
                };
                db.Locations.Add(continent);
                logger.LogInformation("Added country {Continent}", continent.Iso2);
                db.SaveChanges();
            }

            countryIso2ToContinentLocation.Add(cc.Key, continent);
        }

        db.SaveChanges();
        logger.LogInformation("Saving continents");

        List<SimplemapsWorldCity> cities;

        using (var reader = new StreamReader("../_dev/_files/simplemaps-worldcities-basic.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            cities = csv.GetRecords<SimplemapsWorldCity>().ToList().Where(c => c.capital != "minor").ToList();
        }

        foreach (var city in cities)
        {
            var country = db.Locations.Include(location => location.Parent)
                .SingleOrDefault(l => l.Iso2 == city.iso2 && l.Type == LocationType.Country);

            // If country is not in the database
            if (country == null)
            {
                country = new Location
                {
                    Iso2 = city.iso2,
                    Iso3 = city.iso3,
                    Name = city.country,
                    NameAscii = city.country,
                    FullName = city.country,
                    ParentId = countryIso2ToContinentLocation.ContainsKey(city.iso2)
                        ? countryIso2ToContinentLocation[city.iso2]?.Id
                        : null,
                    Type = LocationType.Country
                };
                db.Locations.Add(country);
                logger.LogInformation("Adding country {Country}", country.Name);
                db.SaveChanges();
            }

            var cityParent = country;

            // Handle province if the country is in countriesWithprovinces list
            if (countriesWithProvinces.Contains(city.iso2.ToUpper()))
            {
                var province = db.Locations.Include(location => location.Parent).SingleOrDefault(l =>
                    l.Name == city.admin_name && l.Type == LocationType.Province && l.ParentId == country.Id);

                if (province == null)
                {
                    province = new Location
                    {
                        Name = city.admin_name,
                        NameAscii = city.admin_name,
                        FullName = $"{city.admin_name}, {country.Name}",
                        ParentId = country.Id,
                        Type = LocationType.Province
                    };

                    db.Locations.Add(province);
                    logger.LogInformation("Adding province {Province}", province.Name);
                    db.SaveChanges();
                }

                cityParent = province;
            }
            // Province ends

            var cityFromDb = db.Locations.FirstOrDefault(l =>
                l.NameAscii == city.city_ascii && l.Type == LocationType.City && l.ParentId == cityParent.Id);

            if (cityFromDb == null)
            {
                cityFromDb = new Location
                {
                    Capital = city.capital,
                    Name = city.city,
                    NameAscii = city.city_ascii,
                    FullName = $"{city.city}, {cityParent.Name}",
                    Lat = double.Parse(city.lat),
                    Lng = double.Parse(city.lng),
                    Population = !string.IsNullOrEmpty(city.population) ? (int)double.Parse(city.population) : 0,
                    Parent = cityParent,
                    SimplemapsId = city.id,
                    Type = LocationType.City
                };

                if (cityParent.Parent != null && !string.IsNullOrEmpty(cityParent.Parent.NameAscii))
                    cityFromDb.FullName += $", {cityParent.Parent.Name}";

                db.Locations.Add(cityFromDb);

                logger.LogInformation("Adding city {City}", cityFromDb.Name);
            }
        }

        db.SaveChanges();

        return Ok();
    }

    public async Task<IActionResult> InsertEnglishWords()
    {
        const int batchSize = 1000;
        var filePath = Path.Combine("App_Data", "english_words_alpha.txt");

        await using var db = dbProvider.Create();

        if (db.EnglishWords.Count() > 0) return Ok("English words already imported.");

        using var reader = new StreamReader(filePath);
        var words = new List<EnglishWord>();
        string line;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            words.Add(new EnglishWord { Word = line });

            if (words.Count >= batchSize)
            {
                await db.EnglishWords.AddRangeAsync(words);
                await db.SaveChangesAsync();
                words.Clear(); // Clear the list for the next batch
            }
        }

        // Insert any remaining words
        if (words.Count > 0)
        {
            await db.EnglishWords.AddRangeAsync(words);
            await db.SaveChangesAsync();
        }

        return Ok("Words imported successfully.");
    }

    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private class SimplemapsWorldCity
    {
        public string city { get; }
        public string city_ascii { get; }
        public string lat { get; }

        public string lng { get; }

        // Country name
        public string country { get; }

        // Country iso2
        public string iso2 { get; }

        // Country iso3
        public string iso3 { get; }
        public string admin_name { get; }
        public string capital { get; }
        public string population { get; }
        public string id { get; }
    }
}