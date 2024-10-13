using System.ComponentModel.DataAnnotations;
using WebLibServer.WebSys.MvcAttributes;

namespace SwipetorApp.Areas.Api.Models;

public class UserApiBecomeCreatorReqModel
{
    [Required]
    [MinLength(50)]
    public string Txt { get; set; }

    [ValidateRecaptcha]
    public string RecaptchaValue { get; set; }
}