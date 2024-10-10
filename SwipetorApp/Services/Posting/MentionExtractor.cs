using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace SwipetorApp.Services.Posting;

/// <summary>
///     TODO Update Mention Extractor for React-Mentions
///     Parser example from JS:  txt.replace(/@\[@([^\]]+)]\(([0-9]+)\)/, '@$1');
/// </summary>
public class MentionExtractor(string postHtml)
{
    public List<int> ExtractUserIds()
    {
        var document = new HtmlDocument();
        document.LoadHtml($"<div>{postHtml}</div>");
        var mentions = document.DocumentNode?.SelectNodes("//span[@class='mention']")?.ToList() ?? new List<HtmlNode>();

        var userIds = new List<int>();

        foreach (var mention in mentions)
        {
            var attrs = mention.Attributes.ToDictionary(k => k.Name, v => v.Value);

            // If we are missing related mention data
            if (!attrs.ContainsKey("data-id") || !attrs.ContainsKey("data-value") ||
                !attrs.ContainsKey("data-type") || !attrs.ContainsKey("data-denotation-char"))
                break;

            var id = int.Parse(attrs["data-id"]);
            var value = attrs["data-value"];
            var type = attrs["data-type"];
            var denotChar = attrs["data-denotation-char"];

            if (denotChar == "@" && type == MentionType.User) userIds.Add(id);
        }

        return userIds.Distinct().ToList();
    }
}