using System.ComponentModel.DataAnnotations;
using WebAppShared.WebSys.MvcAttributes;

namespace SwipetorApp.Areas.Api.Models;

public class MyApiProfileDescReqModel
{
    [MaxLength(200)]
    public string Description { get; set; }
}