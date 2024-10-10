using System;
using System.ComponentModel.DataAnnotations;
using WebAppShared.WebSys.MvcAttributes;

namespace SwipetorApp.Areas.Api.Models;

public class AuthEnterLoginCodeReqModel
{
    [Required]
    public Guid LoginRequestId { get; set; }

    [Required]
    [MinLength(5)]
    public string LoginCode { get; set; }

    [ValidateRecaptcha]
    public string RecaptchaValue { get; set; }
}