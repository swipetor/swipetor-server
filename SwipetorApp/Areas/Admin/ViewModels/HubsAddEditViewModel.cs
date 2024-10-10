using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SwipetorApp.Models.DbEntities;
using WebAppShared.WebSys.MvcAttributes;

namespace SwipetorApp.Areas.Admin.ViewModels;

public class HubsAddEditViewModel
{
    public int? Id { get; set; }

    [Required]
    [MaxLength(32)]
    [MinLength(3)]
    public string Name { get; set; }

    [MaxFileSize]
    public IFormFile HubIcon { get; set; }

    public Photo Photo { get; set; }
}