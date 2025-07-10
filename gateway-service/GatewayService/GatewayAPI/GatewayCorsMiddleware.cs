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
        var isOptionsRequest = context.Request.Method == HttpMethods.Options;

        // You can restrict this to specific origins if you prefer
        context.Response.Headers["Access-Control-Allow-Origin"] = origin == "" ? "*" : origin;
        context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS, PATCH";
        context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization, X-Requested-With";
        context.Response.Headers["Access-Control-Allow-Credentials"] = "true";

        if (isOptionsRequest)
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        await _next(context);
    }
}
