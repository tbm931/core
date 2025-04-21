using Microsoft.AspNetCore.Mvc;
using booksProject.Models;

namespace booksProject.Services;

public static class BookServiceStatic
{
    private static List<Book> list;

    static BookServiceStatic()
    {
        list = new List<Book>
        {
            new Book { Id = "1", Name = "פרח בר", AuthorName = "רחל פומרנץ" },
            new Book { Id = "2", Name = "איסתרק", AuthorName = "מיה קינן" },
        };
    }

    public static List<Book> Get()
    {
        return list;
    }

    public static Book? Get(string id)
    {
        var Book = list.FirstOrDefault(b => b.Id == id);
        return Book;
    }

    public static string Insert(Book newBook)
    {
        if (newBook == null
            || string.IsNullOrWhiteSpace(newBook.Name))
            return "-1";

        int maxId = list.Max(p => int.Parse(p.Id));
        newBook.Id = (maxId + 1).ToString();
        list.Add(newBook);

        return newBook.Id;
    }


    public static bool Update(string id, Book newBook)
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

    public static bool Delete(string id)
    {
        var Book = list.FirstOrDefault(p => p.Id == id);
        if (Book == null)
            return false;

        var index = list.IndexOf(Book);
        list.RemoveAt(index);

        return true;
    }

}
