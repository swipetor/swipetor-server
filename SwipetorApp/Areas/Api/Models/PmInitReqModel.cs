using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Areas.Api.Models;

public class PmInitReqModel
{
    [Required]
    [MinLength(1, ErrorMessage = "UserIds must contain at least 1 element.")]
    [MaxLength(3, ErrorMessage = "UserIds can contain a maximum of 3 elements.")]
    public List<int> UserIds { get; set; }
}