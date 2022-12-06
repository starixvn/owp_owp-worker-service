namespace OWP.Infosite.Services;

public interface DateTimeService
{
    DateTime Now { get; }

    DateTime Today { get; }
}

public class DateTimeServiceImp : DateTimeService
{
    public DateTime Now => DateTime.Now;

    public DateTime Today => DateTime.Today;
}