using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Models.EmailViewModels;

public class PmEmailVM
{
    public PmThreadUser PmThreadUser { get; set; }
    public string OtherUsernames { get; set; }
}
