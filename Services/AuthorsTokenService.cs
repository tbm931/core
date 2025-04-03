using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using booksProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

namespace booksProject.Services
{
    public static class AuthorTokenService
    {
        private static SymmetricSecurityKey key
            = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "SXkSqsKyNUyvGbnHs7ke2NCq8zQzNLW7mPmHbnZZ"));
        private static string issuer = "https://fbi-demo.com";
        public static SecurityToken GetToken(List<Claim> claims) =>
            new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.AddDays(30.0),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

        public static TokenValidationParameters
            GetTokenValidationParameters() =>
            new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidAudience = issuer,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };
        public static string GetAuthorNameFromToken(string? token)
        {
            string id = decoder(token);
            var s = GetAuthors();
            return s.First(author => author.Id == id).Name!;
        }
        public static Author GetAuthorFromToken(string? token)
        {
            string id = decoder(token);
            return GetAuthors().First(author => author.Id == id)!;
        }

        public static List<Author> GetAuthors()
        {
            HttpClient client = new HttpClient();
            var fakeHostEnvironment = new FakeHostEnvironment
            {
                EnvironmentName = "Development"
            };
            AuthorJsonService authorJsonService = new AuthorJsonService(fakeHostEnvironment);
            return authorJsonService.Get();
        }
        private static string decoder(string? token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Token is null or empty");
                return "";
            }
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7);
            }

            var handler = new JwtSecurityTokenHandler();
            Console.WriteLine(handler.CanReadToken(token) ? "Token is readable" : "Token is NOT readable");

            try
            {
                if (!handler.CanReadToken(token))
                {
                    Console.WriteLine("Invalid JWT token format");
                    return "";
                }

                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken.Payload.ContainsKey("id"))
                {
                    return jwtToken.Payload["id"].ToString()!;
                }
                else
                {
                    Console.WriteLine("Token does not contain 'id' claim.");
                    return "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding token: {ex.Message}");
                return "";
            }
        }
        public static string WriteToken(SecurityToken token) =>
            new JwtSecurityTokenHandler().WriteToken(token);
    }

    public class FakeHostEnvironment : IHostEnvironment
    {
        public string ApplicationName { get; set; } = "MyTestApp";
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new PhysicalFileProvider(Directory.GetCurrentDirectory());
    }
}