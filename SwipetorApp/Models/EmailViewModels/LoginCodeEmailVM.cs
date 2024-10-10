using SwipetorApp.Models.DbEntities;

namespace SwipetorApp.Models.EmailViewModels;

public class LoginCodeEmailVM
{
    public string Username { get; set; }
    public LoginRequest LoginRequest { get; set; }
}