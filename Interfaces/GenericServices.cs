using booksProject.Models;

namespace booksProject.Interfaces;

public interface IGenericServices<T>
{
    List<T> Get();

    T? Get(string id);

    string Insert(T newT);

    bool Update(string id, T newT);

    bool Delete(string id);
}
