using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SwipetorApp.Areas.Api.Models;

public class DraftPostAddVideoViewModel
{
    [Required]
    public IFormFile File { get; set; }
}