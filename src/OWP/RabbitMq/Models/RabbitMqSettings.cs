namespace OWP.RabbitMq.Models;

public class RabbitMqSetting
{
    public const string RabbitMq = "RabbitMQ";

    public string Hosting { get; set; }

    public string VirtualHost { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public int Port { get; set; }

    public string RequestQueue { get; set; }
}