using booksProject.Models;
using booksProject.Interfaces;
using System.Text.Json;
using System.Reflection;

namespace booksProject.Services
{
    public class GenericJsonService<T> : IGenericServices<T> where T : IdAndName
    {
        public List<T> Ts { get; }
        private static string fileName = $"{typeof(T).Name}s.json";
        private string filePath;
        public GenericJsonService(IHostEnvironment env)
        {
            filePath = Path.Combine(env.ContentRootPath, "Data", fileName);

            using (var jsonFile = File.OpenText(filePath))
            {
                Ts = JsonSerializer.Deserialize<List<T>>(jsonFile.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
        }

        private void saveToFile()
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(Ts));
        }
        public List<T> Get()
        {
            return Ts;
        }

        public T Get(string id) => Ts.FirstOrDefault(t => t.Id == id)!;

        public string Insert(T newT)
        {
            if (newT == null
            || string.IsNullOrWhiteSpace(newT.Name))
                return "-1";
            if (typeof(T).Name == "Book")
                newT.Id = (Ts.Max(au => int.Parse(au.Id)) + 1).ToString();
            Ts.Add(newT);
            saveToFile();
            return newT.Id;
        }

        public bool Delete(string id)
        {
            var t = Get(id);
            if (t is null)
                return false;

            Ts.Remove(t);
            saveToFile();
            return true;
        }

        public bool Update(string id, T newT)
        {
            if (newT == null
                || string.IsNullOrWhiteSpace(newT.Name)
                || newT.Id != id)
                return false;

            var Book = Ts.FirstOrDefault(b => b.Id == id);
            if (Book == null)
                return false;
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (!(prop.Name == "Id" || (typeof(T).Name == "Author" && prop.Name == "Name")))
                {
                    var value = prop.GetValue(newT);
                    prop.SetValue(Book, value);
                }
            }
            saveToFile();

            return true;
        }

        public int Count => Ts.Count();
    }
}