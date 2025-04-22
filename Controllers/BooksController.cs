using Microsoft.AspNetCore.Mvc;
using booksProject.Models;
using Microsoft.AspNetCore.Authorization;
using booksProject.Interfaces;
using booksProject.Services;

namespace BooksController.Controllers;

[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    // private IBookService bookService;

    // public BookController(IBookService bookService)
    // {
    //     this.bookService = bookService;
    // }
    private IGenericServices<Book> bookService;

    public BookController(IGenericServices<Book> bookService)
    {
        this.bookService = bookService;
    }


    [Authorize(Policy = "Author")]
    [HttpGet]
    public ActionResult<IEnumerable<Book>> Get()
    {
        string token = Request.Headers["Authorization"].ToString();
        Author author = AuthorTokenService.GetAuthorFromToken(token);
        if (author.IsAdmin)
            return bookService.Get();
        return bookService.Get().Where(book => book.AuthorName == author.Name).ToList();
    }

    [Authorize(Policy = "Author")]
    [HttpGet("{id}")]
    public ActionResult<Book> Get(string id)
    {
        string token = Request.Headers["Authorization"].ToString();
        Author author = AuthorTokenService.GetAuthorFromToken(token);
        var Book = bookService.Get(id);
        return (author.IsAdmin || Book!.AuthorName == author.Name) ? Book! : Forbid();
    }

    [Authorize(Policy = "Author")]
    [HttpPost]
    public ActionResult Post(Book newBook)
    {
        var newId = bookService.Insert(newBook);
        return CreatedAtAction(nameof(Post), new { Id = newId });
    }

    [Authorize(Policy = "Author")]
    [HttpPut("{id}")]
    public ActionResult Put(string id, Book newBook)
    {
        string token = Request.Headers["Authorization"].ToString();
        Author author = AuthorTokenService.GetAuthorFromToken(token);
        if (author.IsAdmin || author.Name == newBook.AuthorName)
        {
            if (bookService.Update(id, newBook))
            {
                return NoContent();
            }
            throw new ApplicationException("not found");
        }
        return Forbid();
    }

    [Authorize(Policy = "Author")]
    [HttpDelete("{id}")]
    public ActionResult Delete(string id)
    {
        if (bookService.Delete(id))
            return Ok();
        throw new ApplicationException("not found");
    }
}
