namespace OWP.Infosite.Models;

public class IsLiveSynchronizedResult
{
    public List<(int, int?)> FailedMatches { get; set; }

    public List<(int, int?)> SuccessfulMatches { get; set; }

    public string ErrorMessage { get; set; }
}