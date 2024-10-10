using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Areas.Api.Models;

public class DraftPostAddVideoFromUrlViewModel
{
    [Required]
    public int PostId { get; set; }

    [Required]
    [MinLength(5)]
    public string Url { get; set; }
}