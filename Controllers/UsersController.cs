using Microsoft.AspNetCore.Mvc;
using booksProject.Models;
using booksProject.Interfaces;

namespace UsersController.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private IUserService userService;

    public UserController(IUserService bookService)
    {
        this.userService = bookService;
    }
    [HttpGet]
    public ActionResult<IEnumerable<User>> Get()
    {

        return userService.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<User> Get(string id)
    {
        var user = userService.Get(id);
        if (user == null)
            throw new ApplicationException("user not found");
        return user;
    }

    [HttpPost]
    public ActionResult Post(User newUser)
    {
        var newId = userService.Insert(newUser);
        if (newId == -1)
            throw new ApplicationException("didn't success to add");
        return CreatedAtAction(nameof(Post), new { Id = newId });
    }


    [HttpPut("{id}")]
    public ActionResult Put(string id, User newUser)
    {
        if (userService.Update(id, newUser))
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
    public ActionResult Delete(string id)
    {
        if (userService.Delete(id))
            return Ok();
        throw new ApplicationException("not found");
        // return NotFound();
    }
}
