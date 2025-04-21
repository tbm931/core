using Microsoft.AspNetCore.Mvc;
using booksProject.Models;
using booksProject.Interfaces;

namespace booksProject.Services;

public class BookServiceConst : IBookService
{
    private List<Book> list;

    public BookServiceConst()
    {
        list = new List<Book>
        {
            new Book { Id = "1", Name = "פרח בר", AuthorName = "רחל פומרנץ" },
            new Book { Id = "2", Name = "איסתרק", AuthorName = "מיה קינן" },
        };
    }

    public List<Book> Get()
    {
        return list;
    }

    public Book? Get(string id)
    {
        var Book = list.FirstOrDefault(b => b.Id == id);
        return Book;
    }

    public string Insert(Book newBook)
    {
        if (newBook == null
            || string.IsNullOrWhiteSpace(newBook.Name))
            return "-1";

        int maxId = list.Max(p => int.Parse(p.Id));
        newBook.Id = (maxId + 1).ToString();
        list.Add(newBook);

        return newBook.Id;
    }


    public bool Update(string id, Book newBook)
    {
        if (newBook == null
            || string.IsNullOrWhiteSpace(newBook.Name)
            || newBook.Id != id)
        {
            return false;
        }

        var Book = list.FirstOrDefault(p => p.Id == id);
        if (Book == null)
            return false;

        Book.Name = newBook.Name;
        Book.AuthorName = newBook.AuthorName;
        return true;
    }

    public bool Delete(string id)
    {
        var Book = list.FirstOrDefault(p => p.Id == id);
        if (Book == null)
            return false;

        var index = list.IndexOf(Book);
        list.RemoveAt(index);

        return true;
    }

}
public static class BooksUtilities
{
    public static void AddBooksConst(this IServiceCollection services)
    {
        services.AddSingleton<IBookService, BookServiceConst>();
    }

    public static void AddBooksJson(this IServiceCollection services)
    {
        services.AddSingleton<IBookService, BookJsonService>();
    }

    public static void AddAuthorJson(this IServiceCollection services)
    {
        services.AddScoped<IAuthorService, AuthorJsonService>();
    }
}
