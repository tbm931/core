public class LoginRequest
{
    public string? Name { get; set; }
    public string? Id { get; set; }
}

public class AuthorRequest
{
    public bool ifDo { get; set; }
    public string? token { get; set; }
}