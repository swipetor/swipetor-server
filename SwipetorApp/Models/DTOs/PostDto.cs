using System.Collections.Generic;
using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Models.DTOs;

[UsedImplicitly]
public class PostDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public int UserId { get; set; }
    public PublicUserDto User { get; set; }
    public int CommentsCount { get; set; }
    
    protected int FavCount { get; set; }
    
    public bool IsPublished { get; set; }
    
    public bool IsRemoved { get; set; }

    public long CreatedAt { get; set; }

    public bool UserFav { get; set; }

    public List<HubDto> Hubs { get; set; }

    public List<PostMediaDto> Medias { get; set; }
}