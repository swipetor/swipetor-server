using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Services.SqlQueries.Models;

public class UserQueryModel
{
    public User User { get; set; }
    public Photo Photo { get; set; }

    public bool? UserFollows { get; set; }
}