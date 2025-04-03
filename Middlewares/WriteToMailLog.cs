using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace booksProject.Middlewares;
public class WriteToLogMailMiddleware
{
    private RequestDelegate ne;
    public WriteToLogMailMiddleware(RequestDelegate next)
    {
        ne = next;
    }
    public async Task Invoke(HttpContext c)
    {
        var sw = new Stopwatch();
        sw.Start();
        await ne(c);
        string s = $"{c.Request.Path}.{c.Request.Method} took {sw.ElapsedMilliseconds}ms."
            + $" Success: {c.Response.StatusCode == 200}";
        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("המייל ממנו שולחים");
            mail.To.Add("המייל אליו שולחים");
            mail.Subject = "try";
            mail.Body = s;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("המייל ממנו שולחים", "סיסמת האפליקציה של המייל ממנו שולחים");
            smtp.EnableSsl = true;

            smtp.Send(mail);
            Console.WriteLine("המייל נשלח בהצלחה!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"שגיאה: {ex.Message}");
        }
    }
}

public static class WriteToLogMail
{
    public static WebApplication UseWriteToLogMail(this WebApplication a)
    {
        a.UseMiddleware<WriteToLogMailMiddleware>();
        return a;
    }
}