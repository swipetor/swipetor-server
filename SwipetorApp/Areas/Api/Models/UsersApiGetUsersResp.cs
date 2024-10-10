using System.Collections.Generic;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.DTOs;

namespace SwipetorApp.Areas.Api.Models;

public class UsersApiGetUsersResp
{
    public PublicUserDto User { get; set; }
    public List<PostDto> Posts { get; set; }
    public bool CanMsg { get; set; }
    public long? PmThreadId { get; set; }
}