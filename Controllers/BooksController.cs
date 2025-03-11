using Microsoft.AspNetCore.Mvc;
using booksProject.Models;
using booksProject.Interfaces;

namespace BooksController.Controllers;

[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    private IBookService bookService;

    public BookController(IBookService bookService)
    {
        this.bookService = bookService;
    }
    [HttpGet]
    public ActionResult<IEnumerable<Book>> Get()
    {

        return bookService.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<Book> Get(int id)
    {
        var Book = bookService.Get(id);
        // if (Book == null)
        //     return NotFound();
        return Book;
    }

    [HttpPost]
    public ActionResult Post(Book newBook)
    {
        var newId = bookService.Insert(newBook);
        // if (newId == -1)
        // {
        //     return BadRequest();        }

        return CreatedAtAction(nameof(Post), new { Id = newId });
    }


    [HttpPut("{id}")]
    public ActionResult Put(int id, Book newBook)
    {
        if (bookService.Update(id, newBook))
        {
            return NoContent();
        }
        throw new ApplicationException("not found");
        // return BadRequest();

        /*var Book = list.FirstOrDefault(p => p.Id == id);
        if (Book == null)
            return NotFound();*/
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        if (bookService.Delete(id))
            return Ok();
        throw new ApplicationException("not found");
        // return NotFound();
    }
}
