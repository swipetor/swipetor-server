using System;
using JetBrains.Annotations;
using SwipetorApp.Services.Auth;

namespace SwipetorApp.Models.DTOs;

[UsedImplicitly]
public class UserDto
{
    public int Id { get; set; }

    public string Username { get; set; }

    public int CommentCount { get; set; }
    
    public long LastPmCheckAt { get; set; }

    public int UnreadNotifCount { get; set; }

    public Guid? PhotoId { get; set; }
    public virtual PhotoDto Photo { get; set; }

    public string Description { get; set; }

    public string CustomDomain { get; set; }

    public long PremiumUntil { get; set; }

    public UserRole Role { get; set; }
}