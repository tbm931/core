using booksProject.Models;

namespace booksProject.Interfaces;

public interface IUserService
{
    List<User> Get();

    User? Get(string id);

    int Insert(User newUser);

    bool Update(string id, User newUser);

    bool Delete(string id);
}
