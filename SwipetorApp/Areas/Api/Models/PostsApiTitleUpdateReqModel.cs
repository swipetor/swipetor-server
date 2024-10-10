using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Areas.Api.Models;

public class PostsApiTitleUpdateReqModel
{
    [Required]
    [MinLength(3)]
    [MaxLength(128)]
    public string Title { get; set; }
}