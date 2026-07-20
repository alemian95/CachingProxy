using CachingProxy.Models;
using CachingProxy.Services.Args;
using CachingProxy.Services.Cache;
using CachingProxy.Services.Cache.Algorithm;
using CachingProxy.Services.Cache.Storage;

var argsReader = new ArgsReader(args);
string originUrl = argsReader.Read("--origin") ?? "https://dummyjson.com/";
string? portFromArgs = argsReader.Read("--port");
int port = int.TryParse(portFromArgs, out int parsedPort) ? parsedPort : 5123;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICacheStorageDriver<string, Request>, FileStorageDriver<string, Request>>();
builder.Services.AddSingleton<ICacheAlgorithm<string, Request>>(provider => new LruCache<string, Request>(5));
builder.Services.AddSingleton<CacheService<string, Request>>();
builder.Services.AddHttpClient();
builder.WebHost.UseUrls($"http://localhost:{port}");

var app = builder.Build();

if (argsReader.HasFlag("--clear-cache"))
{
    var cacheService = app.Services.GetRequiredService<CacheService<string, Request>>();
    cacheService.Clear();
    return;
}


app.Map("{*catchall}", async (string? catchall, HttpContext context, CacheService<string, Request> cache, IHttpClientFactory clientFactory) =>
{

    var method = context.Request.Method;
    var path = catchall ?? "";
    var completeUrl = $"{originUrl}{path}";

    if (method != "GET")
    {
        Console.WriteLine("Non-GET request forwarded without cache");
        var client = clientFactory.CreateClient();
        var response = await client.GetAsync(completeUrl);
        var body = await response.Content.ReadAsStringAsync();

        return Results.Content(body, statusCode: (int)response.StatusCode);
    }

    try
    {
        Request cached = cache.Read(completeUrl);
        context.Response.Headers.Append("X-Cache", "HIT");
        Console.WriteLine("X-Cache: HIT");
        return Results.Content(cached.ResponseBody, "application/json", statusCode: cached.ResponseStatusCode);
    }
    catch
    {
        Console.WriteLine("X-Cache: MISS");
        context.Response.Headers.Append("X-Cache", "MISS");

        var client = clientFactory.CreateClient();

        var response = await client.GetAsync(completeUrl);
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseStatusCode = (int) response.StatusCode;

        var cachedResponse = new Request
        {
            Url = completeUrl,
            ResponseStatusCode = responseStatusCode,
            ResponseBody = responseBody,
            ResponseHeaders = "application/json"
        };

        cache.Store(completeUrl, cachedResponse);

        return Results.Content(responseBody, "application/json", statusCode: responseStatusCode);
    }
});

app.Run();
