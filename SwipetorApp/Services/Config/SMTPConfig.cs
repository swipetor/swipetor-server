using JetBrains.Annotations;

namespace SwipetorApp.Services.Config;

[UsedImplicitly]
public class SMTPConfig
{
    public int Port { get; set; }
    public bool Encryption { get; set; }
}