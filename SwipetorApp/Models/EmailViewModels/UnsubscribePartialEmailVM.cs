using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Emailing;

namespace SwipetorApp.Models.EmailViewModels;

public class UnsubscribePartialEmailVM
{
    public User User { get; set; }

    public OutgoingEmailType Type { get; set; }
}