using System.Collections.Generic;
using SwipetorApp.Models.DTOs;

namespace SwipetorApp.Areas.Admin.ViewModels;

public class PostsIndexViewModel
{
    public List<PostDto> Posts { get; set; }
    public List<HubDto> Hubs { get; set; }
    public bool IsRemoved { get; set; }
    public int? HubId { get; set; }
}