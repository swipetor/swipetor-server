using JetBrains.Annotations;

namespace SwipetorApp.Areas.Api.Models;

public class PostsGetRequestModel
{
    public int? FirstPostId { get; set; }

    public int? UserId { get; set; }
    
    [CanBeNull]
    public string HubIds { get; set; }
}