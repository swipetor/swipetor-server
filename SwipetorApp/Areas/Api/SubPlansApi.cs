using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipetorApp.Areas.Api.Models;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.DTOs;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Contexts;

namespace SwipetorApp.Areas.Api;

[Authorize]
[ApiController]
[Area(AreaNames.Api)]
[Route("api/sub-plans")]
public class SubPlanApi(IMapper mapper, IDbProvider dbProvider, UserIdCx userIdCx) : Controller
{
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        using var db = dbProvider.Create();
        var plan = db.SubPlans.Where(sp => sp.OwnerUserId == userIdCx.Value)
            .Include(p => p.OwnerUser)
            .SingleOrDefault(p => p.Id == id);

        if (plan == null) return NotFound();

        var planDto = mapper.Map<SubPlanDto>(plan);
        return Json(planDto);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        using var db = dbProvider.Create();
        var plans = db.SubPlans
            .Where(sp => sp.OwnerUserId == userIdCx.Value)
            .ToList();

        var planDtos = mapper.Map<List<SubPlanDto>>(plans);
        return Json(planDtos);
    }

    [HttpPost]
    public IActionResult Create([FromBody] SubPlansUpdateReqModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        using var db = dbProvider.Create();
        var plan = db.SubPlans.FirstOrDefault(p => p.OwnerUserId == userIdCx.Value);

        if (plan == null)
        {
            plan = new SubPlan
            {
                OwnerUserId = userIdCx.Value
            };
            db.SubPlans.Add(plan);
        }

        plan.Name = model.Name;
        plan.Description = model.Description;
        plan.Price = model.Price;
        plan.Currency = model.Currency;

        db.SaveChanges();

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        using var db = dbProvider.Create();
        var plan = db.SubPlans.SingleOrDefault(p => p.Id == id);
        if (plan == null) return NotFound();

        db.SubPlans.Remove(plan);
        db.SaveChanges();

        return NoContent();
    }
}