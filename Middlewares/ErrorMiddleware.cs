namespace booksProject.Middlewares;
public class ErrorMiddleware
{
    private RequestDelegate next;
    public ErrorMiddleware(RequestDelegate next)
    {
        this.next = next;
    }
    public async Task Invoke(HttpContext c)
    {
        c.Items["success"] = false;
        try
        {
            await next(c);
            c.Items["success"] = true;
        }
        catch (ApplicationException ae)
        {
            c.Response.StatusCode = 400;
            await c.Response.WriteAsync(ae.Message);
        }
        catch (Exception e)
        {
            c.Response.StatusCode = 500;
            await c.Response.WriteAsync("פנה לתמיכה הטכנית" + e);
        }
    }
}

public static class Error
{
    public static WebApplication UseError(this WebApplication a)
    {
        a.UseMiddleware<ErrorMiddleware>();
        return a;
    }
}