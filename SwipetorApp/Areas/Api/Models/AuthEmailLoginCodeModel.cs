using System.ComponentModel.DataAnnotations;
using WebAppShared.WebSys.MvcAttributes;

namespace SwipetorApp.Areas.Api.Models;

public class AuthEmailLoginCodeModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [ValidateRecaptcha]
    public string RecaptchaValue { get; set; }
}