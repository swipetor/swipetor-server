using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SwipetorApp.Areas.HostMaster.Models;

public class ScraperSavePostReqModel
{
    [FromForm]
    public IFormFile Video { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Captions { get; set; }

    public string Stats { get; set; }

    public string Info { get; set; }

    public string Article { get; set; }

    public string UrlSlug { get; set; }

    [Required]
    public string ReferenceDomain { get; set; }

    [Required]
    public string ReferenceId { get; set; }

    [CanBeNull]
    public string Comments { get; set; }

    [CanBeNull]
    public string HubIds { get; set; }

    public List<int> HubIdsList => string.IsNullOrWhiteSpace(HubIds) ? new List<int>() : JsonConvert.DeserializeObject<List<int>>(HubIds);
    public List<ScraperSavePostCommentReqModel> CommentsList => string.IsNullOrWhiteSpace(Comments) ? new List<ScraperSavePostCommentReqModel>() : JsonConvert.DeserializeObject<List<ScraperSavePostCommentReqModel>>(Comments);
}

[UsedImplicitly]
public class ScraperSavePostCommentReqModel
{
    public int LikeCount { get; set; }
    public string Original { get; set; }
    public string Rephrased { get; set; }
}