using System.ComponentModel.DataAnnotations;

namespace SwipetorApp.Models.DbEntities;

public class EnglishWord
{
    public int Id { get; set; }

    [MaxLength(128)]
    public string Word { get; set; }
}