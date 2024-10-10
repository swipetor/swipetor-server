using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Models.EmailViewModels;

public class GenericEmailVM
{
    public string Text { get; set; }

    public User User { get; set; }
}