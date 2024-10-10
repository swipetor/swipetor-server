using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Areas.Api.Models;

public class MediaNotifRevealVM
{
    [Required]
    public string Token { get; set; }
}