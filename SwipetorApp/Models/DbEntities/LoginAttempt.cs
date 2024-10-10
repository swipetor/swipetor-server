using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class LoginAttempt
{
    public Guid Id { get; set; }

    public Guid LoginRequestId { get; set; }
    public virtual LoginRequest LoginRequest { get; set; }

    public int? UserId { get; set; }

    [CanBeNull]
    public virtual User User { get; set; }

    [MaxLength(64)]
    public string TriedEmailCode { get; set; }

    [IndexColumn]
    public DateTime CreatedAt { get; set; }

    [IndexColumn]
    [MaxLength(64)]
    public string CreatedIp { get; set; }

    [MaxLength(2048)]
    public string BrowserAgent { get; set; }
}