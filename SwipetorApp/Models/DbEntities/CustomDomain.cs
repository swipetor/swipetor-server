using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace SwipetorApp.Models.DbEntities;

[UsedImplicitly]
public class CustomDomain
{
    public int Id { get; set; }
    
    [IndexColumn]
    [MaxLength(64)]
    [CanBeNull]
    public string DomainName { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }
    
    [MaxLength(128)]
    public string RecaptchaKey { get; set; }
    
    [MaxLength(128)]
    public string RecaptchaSecret { get; set; }
}