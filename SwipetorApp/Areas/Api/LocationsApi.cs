using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/locations")]
public class LocationsApi(IMapper mapper, IDbProvider dbProvider) : Controller
{
    [HttpGet("")]
    public IActionResult Search(string q, LocationType type)
    {
        q = string.Join(" & ", q.Trim().Split(" ").Where(e => !string.IsNullOrEmpty(e)).Select(e => $"{e}:*"));

        using var db = dbProvider.Create();
        var results = db.Locations.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(q)) && p.Type == type)
            .OrderBy(l => l.NameAscii)
            .Take(10).ToList();

        return Json(mapper.Map<List<LocationDto>>(results));
    }
}