namespace SwipetorApp.Models.DTOs;

public class CustomDomainDto
{
    public string DomainName { get; set; }
    public int UserId { get; set; }
    public string RecaptchaKey { get; set; }
    public string RecaptchaSecret { get; set; }
}