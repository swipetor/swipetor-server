using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/search")]
[Authorize]
public class SearchApi(IMapper mapper, IDbProvider dbProvider) : Controller
{
    // TODO Do caching here
    [Route("mention/at/{keyword}")]
    public IActionResult AtMention(string keyword)
    {
        if (keyword.Length < 3) return BadRequest();

        var pattern = $"%{keyword.Trim()}%";

        using var db = dbProvider.Create();
        var matches = db.Users.Where(u => EF.Functions.ILike(u.Username, pattern)).Take(10).ToList();

        var userDtos = mapper.Map<List<UserDto>>(matches);

        return Json(new
        {
            users = userDtos
        });
    }

    // TODO Do caching here
    [Route("mention/hash/{keyword}")]
    public IActionResult HashMention(string keyword)
    {
        if (keyword.Length < 3) return BadRequest();

        var pattern = $"%{keyword.Trim()}%";

        using var db = dbProvider.Create();
        var matches = db.Hubs.Where(u => EF.Functions.ILike(u.Name, pattern)).Take(10).ToList();

        var hubDtos = mapper.Map<List<HubDto>>(matches);

        return Json(new
        {
            labels = hubDtos
        });
    }
}