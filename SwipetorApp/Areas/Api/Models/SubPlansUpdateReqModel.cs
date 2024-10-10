using System;
using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Areas.Api.Models;

public class SubPlansUpdateReqModel
{
    [Required, MinLength(2)] 
    public string Name { get; set; }
    
    [Required, MinLength(10)]
    public string Description { get; set; }
    
    [Required]
    public decimal? Price { get; set; }
    
    [Required]
    public string Currency { get; set; }
}
