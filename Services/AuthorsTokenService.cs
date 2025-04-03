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
                ClockSkew = TimeSpan.Zero // remove delay of token when expire
            };
        public static string GetAuthorNameFromToken(string? token)
        {
            string id = decoder(token);
            var s = GetAuthors();

            System.Console.WriteLine(15);
            System.Console.WriteLine(s);
            // return GetAuthors().First(author => author.Id == id).Name!;
            return s.First(author => author.Id == id).Name!;
        }
        public static Author GetAuthorFromToken(string? token)
        {
            string id = decoder(token);
            System.Console.WriteLine(14);
            System.Console.WriteLine(id);
            return GetAuthors().First(author => author.Id == id)!;
        }

        public static List<Author> GetAuthors()
        {
            System.Console.WriteLine(6);

            HttpClient client = new HttpClient();
            var fakeHostEnvironment = new FakeHostEnvironment
            {
                EnvironmentName = "Development"
            };
            AuthorJsonService authorJsonService = new AuthorJsonService(fakeHostEnvironment);
            System.Console.WriteLine(7);
            return authorJsonService.Get();
        }
        private static string decoder(string? token)
        {
            Console.WriteLine($"Received token: {token}");

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
                Console.WriteLine($"Decoded Token: {jwtToken}");

                Console.WriteLine("Payload:");
                foreach (var claim in jwtToken.Payload)
                {
                    Console.WriteLine($"{claim.Key}: {claim.Value}");
                }

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
        public string EnvironmentName { get; set; } = Environments.Development; // או "Production"
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new PhysicalFileProvider(Directory.GetCurrentDirectory());
    }
}