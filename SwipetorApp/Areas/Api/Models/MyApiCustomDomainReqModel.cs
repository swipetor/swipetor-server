using System.ComponentModel.DataAnnotations;
using WebLibServer.WebSys.MvcAttributes;

namespace SwipetorApp.Areas.Api.Models;

public class MyApiCustomDomainReqModel
{
    [MaxLength(64)]
    [WebDomain]
    public string DomainName { get; set; }

    [MaxLength(128)]
    public string RecaptchaKey { get; set; }

    [MaxLength(128)]
    public string RecaptchaSecret { get; set; }
}
