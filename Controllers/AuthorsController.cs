using Microsoft.AspNetCore.Mvc;
using booksProject.Models;
using booksProject.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using booksProject.Services;

namespace UsersController.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorController : ControllerBase
{
    // private IAuthorService authorService;

    // public AuthorController(IAuthorService authorService)
    // {
    //     this.authorService = authorService;
    // }

    private IGenericServices<Author> authorService;

    public AuthorController(IGenericServices<Author> authorService)
    {
        this.authorService = authorService;
    }

    [HttpPost]
    [Route("[action]")]
    public ActionResult<String> Login([FromBody] LoginRequest loginRequest)
    {
        Author? author = AuthorTokenService.GetAuthors().FirstOrDefault(au => au.Id == loginRequest.Id && au.Name == loginRequest.Name);
        if (author == null)
        {
            return Forbid();
        }
        var _claims = new List<Claim>();

        if (author.IsAdmin)
        {
            _claims = new List<Claim>
            {
                new Claim("type", "Admin"),
                new Claim("id",author.Id!)
            };
        }
        else
        {
            _claims = new List<Claim>
            {
                new Claim("type", "Author"),
                new Claim("id",author.Id!)
            };
        }

        var token = AuthorTokenService.GetToken(_claims);
        return new OkObjectResult(AuthorTokenService.WriteToken(token));
    }

    [Authorize(Policy = "Admin")]
    [HttpGet]
    public ActionResult<IEnumerable<Author>> Get()
    {

        return authorService.Get();
    }
    [Authorize(Policy = "Author")]
    [HttpGet("{id}")]
    public ActionResult<Author> Get(string id)
    {
        var user = authorService.Get(id);
        if (user == null)
            throw new ApplicationException("user not found");
        string token = Request.Headers["Authorization"].ToString();
        Author author = AuthorTokenService.GetAuthorFromToken(token);
        if (author.IsAdmin || author.Id == user.Id)
            return user;
        return Forbid();
    }
    [Authorize(Policy = "Author")]
    [HttpPost]
    public ActionResult Post(Author newAuthor)
    {
        string token = Request.Headers["Authorization"].ToString();
        Author author = AuthorTokenService.GetAuthorFromToken(token);
        if (author.IsAdmin)
        {
            var newId = authorService.Insert(newAuthor);
            if (newId == "null object")
                throw new ApplicationException("didn't success to add");
            return CreatedAtAction(nameof(Post), new { Id = newId });
        }
        else if (author.Id == newAuthor.Id)
        {
            newAuthor.IsAdmin = author.IsAdmin;
            var newId = authorService.Insert(newAuthor);
            if (newId == "null object")
                throw new ApplicationException("didn't success to add");
            return CreatedAtAction(nameof(Post), new { Id = newId });
        }
        else
            return Forbid();
    }

    [Authorize(Policy = "Author")]
    [HttpPut("{id}")]
    public ActionResult Put(string id, Author newAuthor)
    {
        System.Console.WriteLine(newAuthor.Id + " " + newAuthor.Name + " " + newAuthor.Phone + " " + newAuthor.IsAdmin);
        string token = Request.Headers["Authorization"].ToString();
        Author author = AuthorTokenService.GetAuthorFromToken(token);
        if (author.IsAdmin || author.Id == id)
        {
            System.Console.WriteLine("here");
            if (authorService.Update(id, newAuthor))
            {
                System.Console.WriteLine("here2");
                return NoContent();
            }
            throw new ApplicationException("not found");
        }
        return Forbid();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(string id)
    {
        if (authorService.Delete(id))
            return Ok();
        throw new ApplicationException("not found");
    }

    [HttpPost("GetAuthorFromT")]
    public Author? GetAuthorFromT([FromBody] AuthorRequest authorRequest)
    {
        if (authorRequest.ifDo)
            return AuthorTokenService.GetAuthorFromToken(authorRequest.token);
        return null;
    }
}
