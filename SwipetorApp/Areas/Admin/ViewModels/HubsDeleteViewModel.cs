using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Areas.Admin.ViewModels;

public class HubsDeleteViewModel
{
    public Hub Hub { get; set; }
    public List<Tuple<int?, string>> HubList { get; set; }

    [Required]
    public int? HubIdToDelete { get; set; }

    [Required]
    public int? HubIdToMovePosts { get; set; }
}