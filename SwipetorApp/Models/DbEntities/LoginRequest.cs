using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class LoginRequest
{
    public Guid Id { get; set; }

    [IndexColumn]
    [MaxLength(128)]
    public string Email { get; set; }

    public int? UserId { get; set; }

    [CanBeNull]
    public virtual User User { get; set; }

    [IndexColumn]
    [MaxLength(32)]
    public string EmailCode { get; set; }

    [IndexColumn]
    public bool IsUsed { get; set; }

    [IndexColumn]
    public DateTime CreatedAt { get; set; }

    [IndexColumn]
    [MaxLength(64)]
    public string CreatedIp { get; set; }

    [MaxLength(2048)]
    public string BrowserAgent { get; set; }
}