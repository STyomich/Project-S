using System.Diagnostics;
using System.Text;

namespace UsersService.API.Middlewares;

    public class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Log Request details
        var (ipAddress, url, method, requestBody) = await LogRequest(context);

        // Copy the original response body stream to capture the response content
        var originalResponseBodyStream = context.Response.Body;
        using var newResponseBodyStream = new MemoryStream();
        context.Response.Body = newResponseBodyStream;

        try
        {
            await _next(context);

            // Log Response details
            var (statusCode, responseBody) = await LogResponse(context, newResponseBodyStream);

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // Combine and log the request/response details
            _logger.LogInformation(
                "Request: IP Address: {IpAddress}, Target URL: {Url}, HTTP Method: {Method}, Request Content: {RequestBody}", ipAddress, url, method, requestBody);

            _logger.LogInformation(
                "Response: Status Code: {StatusCode}, Response Content: {ResponseBody}, Elapsed Time: {ElapsedMs}ms", statusCode, responseBody, elapsedMs);
        }
        finally
        {
            // Copy the content of the new response body back to the original response body stream
            newResponseBodyStream.Seek(0, SeekOrigin.Begin);
            await newResponseBodyStream.CopyToAsync(originalResponseBodyStream);
            context.Response.Body = originalResponseBodyStream;
        }
    }

    private static async Task<(string? IpAddress, string Url, string Method, string? RequestBody)> LogRequest(HttpContext context)
    {
        var request = context.Request;

        // Retrieve IP Address
        string? ipAddress = context.Connection.RemoteIpAddress?.ToString();

        // Retrieve target URL and method
        var targetUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        var method = request.Method;

        // Read request body (enable buffering to access the body multiple times)
        string? requestBody = null;
        if (request.ContentLength > 0 && request.Body != null)
        {
            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            request.Body.Position = 0; // Reset the stream position
        }

        return (ipAddress, targetUrl, method, requestBody);
    }

    private static async Task<(int StatusCode, string ResponseBody)> LogResponse(HttpContext context, MemoryStream responseBodyStream)
    {
        var response = context.Response;

        responseBodyStream.Seek(0, SeekOrigin.Begin);

        // Read response content
        string responseBody = await new StreamReader(responseBodyStream, Encoding.UTF8).ReadToEndAsync();

        responseBodyStream.Seek(0, SeekOrigin.Begin); // Reset stream position

        return (response.StatusCode, responseBody);
    }
}
