using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Services.SqlQueries.Models;

public class PmThreadUserQueryModel
{
    public PmThreadUser ThreadUser { get; set; }
    public UserQueryModel UserQueryModel { get; set; }
}
