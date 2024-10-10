using System.Collections.Generic;
using JetBrains.Annotations;
using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Services.SqlQueries.Models;

public class PostQueryModel
{
    public Post Post { get; set; }
    [CanBeNull]
    public User User { get; set; }
    public Photo UserPhoto { get; set; }
    public bool UserFollows { get; set; }
    public bool UserFav { get; set; }
    public ICollection<PostMedia> Medias { get; set; }
}