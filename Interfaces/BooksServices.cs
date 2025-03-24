using booksProject.Models;

namespace booksProject.Interfaces;

public interface IBookService
{
    List<Book> Get();

    Book? Get(int id);

    int Insert(Book newBook);
    
    bool Update(int id, Book newBook);

    bool Delete(int id);
}
