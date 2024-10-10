using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Areas.Api.Models;

public class RegisterPushDeviceRequestModel
{
    [Required] public string Token { get; set; }
}
