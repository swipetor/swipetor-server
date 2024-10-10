using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Areas.Api.Models;

public class CommentReqModel
{
    [Required]
    [MinLength(3)]
    public string Txt { get; set; }

    [Required]
    public int PostId { get; set; }
}