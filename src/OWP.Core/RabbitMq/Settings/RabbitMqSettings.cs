namespace OWP.Core.RabbitMq.Settings;

public class RabbitMqSettings
{
    public string Hosting { get; set; }
    public string VirtualHost { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public string Exchange { get; set; }
    public string Queue { get; set; }
}