using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MoreLinq;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.SqlQueries;
using Z.EntityFramework.Plus;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("/api/hubs")]
public class HubsApi(IDbProvider dbProvider, IMapper mapper, HubSvc hubSvc) : Controller
{
    public JsonResult Index()
    {
        using var db = dbProvider.Create();

        var hubPostCounts = db.Hubs.Select(c => new
        {
            HubId = c.Id,
            PostCount = c.PostHubs.Count
        }).FromCache(new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(1)
        }).ToDictionary(k => k.HubId, v => v.PostCount);

        var hubs = db.Hubs.SelectForUser().ToList();

        var hubDtos = mapper.Map<List<HubDto>>(hubs);
        var hubsById = hubDtos.ToDictionary(f => f.Id);

        hubsById.ForEach(c => c.Value.PostCount = hubPostCounts.TryGetValue(c.Key, out var count) ? count : 0);

        return Json(new
        {
            hubsById
        });
    }

    [Route("{id}")]
    public JsonResult Get(int id)
    {
        if (id <= 0) return Json(hubSvc.GetById(id));

        using var db = dbProvider.Create();

        var hub = db.Hubs.Where(f => f.Id == id).SelectForUser().Single();

        return Json(mapper.Map<HubDto>(hub));
    }

    [HttpGet("search")]
    public IActionResult Search(string name)
    {
        using var db = dbProvider.Create();
        var hubs = db.Hubs.Where(c => EF.Functions.ILike(c.Name, $"%{name.Trim()}%")).OrderBy(h => h.Name)
            .Take(10).ToList();

        var hubDtos = mapper.Map<List<HubDto>>(hubs);
        return Json(hubDtos);
    }
}