using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Areas.Api.Models;

public class PmInviteRequestModel
{
    [Required] public int UserId { get; set; }

    [Required]
    [MinLength(10)]
    [MaxLength(120)]
    public string Intro { get; set; }
}