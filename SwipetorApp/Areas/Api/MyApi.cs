using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Areas.Api.Models;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.PhotoServices;
using SwipetorApp.Services.Users;
using SwipetorApp.System;
using WebLibServer.DI;
using WebLibServer.Exceptions;
using WebLibServer.WebSys;
using WebLibServer.WebSys.MvcAttributes;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/my")]
[Authorize]
public class MyApi(
    IFactory<PhotoSaverSvc> photoSaverFactory,
    IMapper mapper,
    UserCx userCx,
    IDbProvider dbProvider,
    PhotoDeleterSvc photoDeleterSvc)
    : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        var userId = userCx.ValueOrNull?.Id;

        using var db = dbProvider.Create();
        var user = db.Users.Include(u => u.Photo).Where(u => u.Id == userId)
            .SingleOrDefault();
        var userDto = mapper.Map<UserDto>(user);

        return Json(new
        {
            user = userDto
        });
    }

    [AllowAnonymous]
    [Route("ping")]
    public IActionResult Ping()
    {
        if (!userCx.IsLoggedIn) return Ok();

        using var db = dbProvider.Create();

        var unreadNotifCount = db.Notifs.Where(n => n.ReceiverUserId == userCx.Value.Id && n.IsViewed == false)
            .Count();
        
        var unreadPmCount = db.PmThreadUsers.Where(tu =>
                tu.UserId == userCx.Value.Id && tu.UnreadMsgCount > 0 &&
                tu.Thread.LastMsgAt > tu.User.LastPmCheckAt)
            .Count();

        var user = db.Users.Include(u => u.Photo).Where(u => u.Id == userCx.Value.Id).SingleOrDefault();

        var userDto = mapper.Map<UserDto>(user);

        return Json(new
        {
            unreadNotifCount,
            unreadPmCount,
            uiVersion = DeployInfo.GetUiVersion(),
            appVersion = DeployInfo.GetAppVersion(),
            user = userDto
        });
    }

    [HttpPost]
    [Route("photos")]
    public async Task<IActionResult> Photos([MaxFileSize] IFormFile file)
    {
        using var photoSaver = photoSaverFactory.GetInstance()
            .SetSource(file.OpenReadStream())
            .SetExtensionByFileName(file.FileName)
            .SetMaxWidthHeight(1280);

        var photoUploadingTask = photoSaver.Save();

        await using var db = dbProvider.Create();
        var user = db.Users.Single(u => u.Id == userCx.Value.Id);

        // Delete if photo exists
        if (user.PhotoId != null) await photoDeleterSvc.Delete(user.PhotoId.Value);

        var photo = await photoUploadingTask;

        user.PhotoId = photo.Id;
        await db.SaveChangesAsync();

        return Ok();
    }
    
    [HttpPut("profile")]
    [Authorize]
    public IActionResult Profile(MyApiProfileDescReqModel model)
    {
        if (!ModelState.IsValid) throw new HttpJsonError("Data is invalid.");
        
        using var db = dbProvider.Create();
        var user = db.Users.Single(u => u.Id == userCx.Value.Id);
        user.Description = !string.IsNullOrWhiteSpace(model.Description) ? model.Description.Trim() : null;
        db.SaveChanges();

        return Ok();
    }
    
    [HttpGet("custom-domain")]
    [Authorize]
    [MinRole(UserRole.Creator)]
    public IActionResult CustomDomain() 
    {
        using var db = dbProvider.Create();

        var customDomain = db.CustomDomains.Where(cd => cd.UserId == userCx.Value.Id).SingleOrDefault();
        var dto = mapper.Map<CustomDomainDto>(customDomain);
        return Json(dto);
    }
    
    [HttpPut("custom-domain")]
    [Authorize]
    [MinRole(UserRole.Creator)]
    public IActionResult CustomDomain(MyApiCustomDomainReqModel model)
    {
        if (!ModelState.IsValid) throw new HttpJsonError("Data is invalid.");
        
        using var db = dbProvider.Create();
        
        if (db.CustomDomains.Any(u => u.DomainName == model.DomainName && u.UserId != userCx.Value.Id))
            throw new HttpJsonError("Custom domain is already taken.");
        
        var cd = db.CustomDomains.SingleOrDefault(c => c.UserId == userCx.Value.Id);

        if (cd == null)
        {
            cd = new CustomDomain()
            {
                UserId = userCx.Value.Id
            };
            db.CustomDomains.Add(cd);
        }
        
        cd.DomainName = model.DomainName;
        cd.RecaptchaKey = model.RecaptchaKey;
        cd.RecaptchaSecret = model.RecaptchaSecret;
        
        db.SaveChanges();
        
        return Ok();
    }
    
    [HttpDelete]
    [Route("photos/{photoId}")]
    public IActionResult Photos(Guid photoId)
    {
        using var db = dbProvider.Create();
        var photo = db.Users.Include(user => user.Photo).Single(u => u.Id == userCx.Value.Id).Photo;

        if (photo == null) throw new HttpStatusCodeException(HttpStatusCode.NotFound);

        db.Photos.Remove(photo);
        db.SaveChanges();

        return Ok();
    }
}