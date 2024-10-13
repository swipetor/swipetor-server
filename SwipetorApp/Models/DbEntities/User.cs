using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using SwipetorApp.Services.Auth;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebLibServer.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class User : IDbEntity
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(128)]
    [IndexColumn]
    public string Email { get; set; }

    [CanBeNull]
    [MinLength(3)]
    [MaxLength(18)]
    [IndexColumn(IsUnique = true)]
    public string Username { get; set; }

    [Required]
    [MaxLength(16)]
    public string Secret { get; set; }
    public int CommentCount { get; set; }

    [IndexColumn]
    public UserRole Role { get; set; } = UserRole.Default;
    
    [IndexColumn] 
    public DateTime LastPmCheckAt { get; set; }
    [IndexColumn] 
    public DateTime LastNotifCheckAt { get; set; }
    [IndexColumn] 
    public DateTime LastNotifEmailAt { get; set; }
    [IndexColumn]
    public DateTime LastOnlineAt { get; set; }

    [MaxLength(64)]
    [CanBeNull]
    public string LastOnlineIp { get; set; }
    
    /// <summary>
    ///     Send PM Reminder email if for how long the user has not checked the PM for
    ///     0 is instant
    ///     null is no email
    /// </summary>
    [IndexColumn]
    public int? PmEmailIntervalHours { get; set; } = 3;

    /// <summary>
    ///     Send Notification reminder email if for how long the user has not checked the notifications for
    ///     0 is instant
    ///     null is no email
    /// </summary>
    [IndexColumn]
    public int? NotifEmailIntervalHours { get; set; } = 24;

    public Guid? PhotoId { get; set; }

    public virtual Photo Photo { get; set; }
    
    [MaxLength(200)]
    public string Description { get; set; }

    [MaxLength(200), CanBeNull]
    public string RobotSource { get; set; }
    
    public virtual CustomDomain CustomDomain { get; set; }

    public DateTime? PremiumUntil { get; set; }

    public DateTime CreatedAt { get; set; }

    [MaxLength(64)]
    [IndexColumn]
    public string CreatedIp { get; set; }
    
    [MaxLength(64)]
    [IndexColumn]
    public string ModifiedIp { get; set; }

    [MaxLength(2048)]
    public string BrowserAgent { get; set; }
    
    public virtual ICollection<UserFollow> Followers { get; set; }
    public virtual ICollection<UserFollow> Following { get; set; }
    public virtual ICollection<Notif> Notifs { get; set; }
    public virtual ICollection<Sub> Subscriptions { get; set; }
    public virtual ICollection<SubPlan> OwnedSubscriptionPlans { get; set; }
    public virtual ICollection<Post> Posts { get; set; }
    public virtual ICollection<PmThreadUser> PmThreadUsers { get; set; }
    public virtual ICollection<PushDevice> PushDevices { get; set; }
}