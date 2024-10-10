using System.Collections.Generic;

namespace SwipetorApp.Models.ViewModels;

public class ErrorPartialViewModel
{
    public string Title { get; set; }
    public List<string> Errors { get; set; } = [];
}