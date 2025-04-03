using System.Diagnostics;
using booksProject.Services;

namespace booksProject.Middlewares;
public class WriteToLogMiddleware
{
    private RequestDelegate ne;
    public WriteToLogMiddleware(RequestDelegate next)
    {
        ne = next;
    }
    public async Task Invoke(HttpContext c)
    {
        var token = c.Request.Headers["Authorization"].ToString();
        if (token == "")
            await ne(c);
        else
        {
            var author = AuthorTokenService.GetAuthorFromToken(token);
            var sw = new Stopwatch();
            sw.Start();
            await ne(c);
            string s = $"{c.Request.Path}.{c.Request.Method} took {sw.ElapsedMilliseconds}ms.\n User: {author.Name} UserType: {(author.IsAdmin ? "מנהל" : "סופר")} \n Success: {c.Response.StatusCode == 200}";
            using (StreamWriter stw = new StreamWriter(@"C:\Users\user1\Documents\GitHub\core\Data\Log.txt", true))
                stw.WriteLine(s);
        }
    }
}

public static class WriteToLog
{
    public static WebApplication UseWriteToLog(this WebApplication a)
    {
        a.UseMiddleware<WriteToLogMiddleware>();
        return a;
    }
}