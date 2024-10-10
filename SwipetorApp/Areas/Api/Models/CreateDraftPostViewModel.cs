using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Areas.Api.Models;

public class CreateDraftPostViewModel
{
    [MinLength(3)]
    [Required]
    public string Title { get; set; }
}