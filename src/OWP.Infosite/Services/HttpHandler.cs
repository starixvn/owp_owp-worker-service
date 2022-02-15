namespace OWP.Infosite.Services;

public interface HttpHandler
{
    Task<HttpResponseMessage> GetAsync(string requestLink);
}

public class HttpHandlerImp : HttpHandler, IDisposable
{
    private const int timeout = 30;

    private static readonly HttpClient client = CreateHttpClientHandler();

    public static HttpClient CreateHttpClientHandler()
        => new HttpClient { Timeout = TimeSpan.FromMinutes(timeout) };

    public async Task<HttpResponseMessage> GetAsync(string requestLink)
        => await client.GetAsync(new Uri(requestLink));

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            client?.Dispose();
        }
    }
}