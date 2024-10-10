using System;
using JetBrains.Annotations;
using SwipetorApp.Services.Auth;

namespace SwipetorApp.Models.DTOs;

[UsedImplicitly]
public class PublicUserDto
{
    public int Id { get; set; }

    public string Username { get; set; }
    
    public Guid? PhotoId { get; set; }
    public virtual PhotoDto Photo { get; set; }

    public string Description { get; set; }
    
    /// <summary>
    /// If this user is followed by the current user
    /// </summary>
    public bool UserFollows { get; set; }
}