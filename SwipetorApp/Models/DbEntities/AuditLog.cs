using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using SwipetorApp.Models.Enums;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using WebAppShared.Types;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class AuditLog : IDbEntity
{
    public Guid Id { get; set; }

    [IndexColumn]
    public DateTime CreatedAt { get; set; }

    [IndexColumn, MaxLength(128)]
    public string EntityName { get; set; }

    [MaxLength(64)]
    [IndexColumn]
    public string EntityId { get; set; }

    /// <summary>
    ///     The user who performed the action
    /// </summary>
    public int UserId { get; set; }
    public virtual User User { get; set; }

    [IndexColumn]
    public AuditAction Action { get; set; }

    [MaxLength(4096)]
    public string Log { get; set; }

    [IndexColumn]
    [MaxLength(64)]
    public string CreatedIp { get; set; }

    [MaxLength(2048)]
    public string BrowserAgent { get; set; }
}