using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Areas.Api.Models;

public class AuthSetUsernameRequestModel
{
    [Required]
    [MinLength(3)]
    [MaxLength(18)]
    public string Username { get; set; }
}