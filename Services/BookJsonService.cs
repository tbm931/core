using booksProject.Models;
using booksProject.Interfaces;
using System.Text.Json;

namespace booksProject.Services
{
    public class BookJsonService : IBookService
    {
        public List<Book> books { get; }
        private static string fileName = "Books.json";
        private string filePath;
        public BookJsonService(IHostEnvironment env)
        {
            filePath = Path.Combine(env.ContentRootPath, "Data", fileName);

            using (var jsonFile = File.OpenText(filePath))
            {
                books = JsonSerializer.Deserialize<List<Book>>(jsonFile.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
        }

        private void saveToFile()
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(books));
        }
        public List<Book> Get()
        {
            return books;
        }

        public Book Get(int id) => books.FirstOrDefault(b => b.Id == id)!;

        public int Insert(Book newBook)
        {
            if (newBook == null
            || string.IsNullOrWhiteSpace(newBook.Name))
                return -1;
            newBook.Id = books.Max(au => au.Id) + 1;
            books.Add(newBook);
            saveToFile();
            return newBook.Id;
        }

        public bool Delete(int id)
        {
            var Book = Get(id);
            if (Book is null)
                return false;

            books.Remove(Book);
            saveToFile();
            return true;
        }

        public bool Update(int id, Book newBook)
        {
            if (newBook == null
                || string.IsNullOrWhiteSpace(newBook.Name)
                || newBook.Id != id)
            {
                return false;
            }

            var Book = books.FirstOrDefault(b => b.Id == id);
            if (Book == null)
                return false;

            Book.Name = newBook.Name;
            saveToFile();

            return true;
        }

        public int Count => books.Count();
    }
}