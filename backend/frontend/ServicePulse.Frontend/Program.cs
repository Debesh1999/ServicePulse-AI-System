using System.Net;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// API backend the React SPA talks to (relative /api/* requests are proxied here)
var apiBaseUrl = (Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://localhost:5295").TrimEnd('/');

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

var app = builder.Build();

// No UseHttpsRedirection — we run locally over HTTP only.

// Proxy MUST come before UseStaticFiles so /api/*
// requests are forwarded directly to the backend without redirecting.
app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
    subApp =>
    {
        subApp.Run(async context =>
        {
            var queryString = context.Request.QueryString.HasValue
                ? context.Request.QueryString.ToUriComponent()
                : string.Empty;
            var targetUrl = apiBaseUrl + context.Request.Path + queryString;
            using var httpClient = new HttpClient();
            var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUrl);

            foreach (var header in context.Request.Headers)
            {
                if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase)
                    || header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                    continue;
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            byte[]? bodyBytes = null;
            if (context.Request.ContentLength > 0 || context.Request.HasFormContentType)
            {
                bodyBytes = await ReadBodyAsync(context.Request.Body);
                requestMessage.Content = new ByteArrayContent(bodyBytes);

                if (context.Request.ContentType != null)
                {
                    requestMessage.Content.Headers.ContentType =
                        System.Net.Http.Headers.MediaTypeHeaderValue.Parse(context.Request.ContentType);
                }

                foreach (var header in context.Request.Headers)
                {
                    if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase)
                        || header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)
                        || header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                        continue;
                    requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            try
            {
                using var response = await httpClient.SendAsync(requestMessage, context.RequestAborted);
                context.Response.StatusCode = (int)response.StatusCode;
                foreach (var header in response.Headers)
                {
                    if (header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                        continue;
                    context.Response.Headers.Append(header.Key, header.Value.ToArray());
                }
                foreach (var header in response.Content.Headers)
                {
                    context.Response.Headers.Append(header.Key, header.Value.ToArray());
                }
                await response.Content.CopyToAsync(context.Response.Body);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(new { message = "API proxy error: " + ex.Message }), Encoding.UTF8);
            }
        });
    });

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();

static async Task<byte[]> ReadBodyAsync(Stream body)
{
    using var ms = new MemoryStream();
    await body.CopyToAsync(ms);
    ms.Position = 0;
    return ms.ToArray();
}
