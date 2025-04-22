namespace booksProject.Models;

public class Author : IdAndName
{
    public string? Phone { get; set; }
    public bool IsAdmin { get; set; }
}
