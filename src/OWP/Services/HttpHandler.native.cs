namespace OWP.Services;

public interface HttpHandler
{
    Task<HttpResponseMessage> GetAsync(string requestLink);

    Task<HttpResponseMessage> PostAsync(string requestLink, HttpContent content);
}

public class HttpHandlerImp : HttpHandler, IDisposable
{
    private const int timeout = 30;

    private static readonly HttpClient client = CreateHttpClientHandler();

    public static HttpClient CreateHttpClientHandler() => new() { Timeout = TimeSpan.FromMinutes(timeout) };

    public async Task<HttpResponseMessage> GetAsync(string requestLink)
        => await client.GetAsync(new Uri(requestLink));

    public async Task<HttpResponseMessage> PostAsync(string requestLink, HttpContent content)
        => await client.PostAsync(new Uri(requestLink), content);

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