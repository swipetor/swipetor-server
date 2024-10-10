using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Services.SqlQueries.Models;

public class CommentQueryModel
{
    public Comment Comment { get; set; }
    public Photo UserPhoto { get; set; }
    public User User { get; set; }
}