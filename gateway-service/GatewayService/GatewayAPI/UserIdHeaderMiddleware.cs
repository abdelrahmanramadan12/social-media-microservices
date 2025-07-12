using System.Text.Json;

public class UserIdHeaderMiddleware
{
    private readonly RequestDelegate _next;
    public UserIdHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        if (path != null && !path.Contains("api/public/auth") && !path.Contains("chathub") && !path.Contains("notificationhub"))
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { errors = new[] { "Unauthorized: Missing or invalid Authorization header" } }));
                return;
            }
            var token = authHeader.Substring("Bearer ".Length).Trim();
            try
            {
                using var httpClient = new HttpClient();
                var verifyRequest = new HttpRequestMessage(HttpMethod.Get, "http://auth:8080/verify");
                verifyRequest.Headers.Add("Authorization", $"Bearer {token}");
                var verifyResponse = await httpClient.SendAsync(verifyRequest);
                if (!verifyResponse.IsSuccessStatusCode)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new { errors = new[] { "Unauthorized: Invalid token" } }));
                    return;
                }
                var json = await verifyResponse.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("id", out var idProp) || idProp.ValueKind != JsonValueKind.String)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new { errors = new[] { "Unauthorized: Invalid response from auth service" } }));
                    return;
                }
                var id = idProp.GetString();
                context.Request.Headers["userId"] = id;
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { errors = new[] { "Unauthorized: Token verification error" } }));
                return;
            }
        }
        await _next(context);
    }
}
