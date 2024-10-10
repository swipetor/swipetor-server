using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAppShared.WebSys.MvcAttributes;

namespace SwipetorApp.Areas.HostMaster.Models;

public class ScraperSaveUserReqModel
{
    [Required, MinLength(3)]
    public string Username { get; set; }

    [FromForm, MaxFileSize(2), CanBeNull]
    public IFormFile Photo { get; set; }

    [Required, MaxLength(200)]
    public string RobotSource { get; set; }
}
