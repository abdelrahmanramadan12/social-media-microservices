using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class GatewayCorsMiddleware
{
    private readonly RequestDelegate _next;

    public GatewayCorsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var origin = context.Request.Headers["Origin"].ToString();

        context.Response.Headers["Access-Control-Allow-Origin"] = !string.IsNullOrEmpty(origin) ? origin : "*";
        context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS, PATCH";
        context.Response.Headers["Access-Control-Allow-Headers"] =
            "Content-Type, Authorization, X-Requested-With, X-SignalR-User-Agent";
        context.Response.Headers["Access-Control-Allow-Credentials"] = "true";

        if (context.Request.Method == HttpMethods.Options)
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        await _next(context);
    }
}
