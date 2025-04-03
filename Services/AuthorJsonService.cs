using booksProject.Models;
using booksProject.Interfaces;
using System.Text.Json;

namespace booksProject.Services
{
    public class AuthorJsonService : IAuthorService
    {
        public List<Author> authors { get; }
        private static string fileName = "Authors.json";
        private string filePath;
        public AuthorJsonService(IHostEnvironment env)
        {
            filePath = Path.Combine(env.ContentRootPath, "Data", fileName);

            using (var jsonFile = File.OpenText(filePath))
            {
                authors = JsonSerializer.Deserialize<List<Author>>(jsonFile.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
        }

        private void saveToFile()
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(authors));
        }

        public List<Author> Get()
        {
            System.Console.WriteLine(8);
            System.Console.WriteLine(authors);
            return authors;
        }

        public Author Get(string id) => authors.FirstOrDefault(a => a.Id == id)!;

        public string Insert(Author newAuthor)
        {
            if (newAuthor == null
            || string.IsNullOrWhiteSpace(newAuthor.Name))
                return "null object";
            authors.Add(newAuthor);
            saveToFile();
            return newAuthor.Id!;
        }

        public bool Delete(string id)
        {
            var Author = Get(id);
            if (Author is null)
                return false;

            authors.Remove(Author);
            saveToFile();
            return true;
        }

        public bool Update(string id, Author newAuthor)
        {
            if (newAuthor == null
                || newAuthor.Id != id)
            {
                return false;
            }

            var Author = authors.FirstOrDefault(a => a.Id == id);
            if (Author == null)
                return false;

            Author.Name = newAuthor.Name;
            Author.Phone = newAuthor.Phone;
            Author.IsAdmin = newAuthor.IsAdmin;
            saveToFile();

            return true;
        }

        public int Count => authors.Count();
    }
}