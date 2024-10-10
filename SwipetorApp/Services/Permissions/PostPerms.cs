using SwipetorApp.Models.DbEntities;
using SwipetorApp.Services.Auth;
using WebAppShared.Exceptions;

namespace SwipetorApp.Services.Permissions;

public class PostPerms
{
    public bool CanEdit(Post post, User user)
    {
        if (post.UserId == user.Id) return true;

        return user.Role >= UserRole.Editor;
    }

    public void AssertCanEdit(Post post, User user)
    {
        if (!CanEdit(post, user)) throw new HttpJsonError("Not enough permission");
    }

    public bool CanDelete(Post post, User user)
    {
        return post.UserId == user.Id || user.Role >= UserRole.Editor;
    }

    public void AssertCanDelete(Post post, User user)
    {
        if (!CanDelete(post, user)) throw new HttpJsonError("Not enough permission");
    }
}