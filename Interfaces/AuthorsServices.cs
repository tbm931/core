using booksProject.Models;

namespace booksProject.Interfaces;

public interface IAuthorService
{
    List<Author> Get();

    Author? Get(string id);

    string Insert(Author newAuthor);

    bool Update(string id, Author newAuthor);

    bool Delete(string id);
}
