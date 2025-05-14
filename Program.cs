using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("password123-password123-password123-password123"))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter your JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();


app.MapPost("/login", (string username, string password) =>
{
    if (username!= "admin" || password != "password")
    {
        return Results.Unauthorized();
    }
    var claims = new[] { new Claim(ClaimTypes.Name, username) };
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("password123-password123-password123-password123"));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
       claims: claims,
       expires: DateTime.UtcNow.AddMinutes(60),
       signingCredentials: creds
   );
    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = tokenString });
});


var books = new List<Book>
{
    new Book { Id = 1, Title = "1984", Author = "George Orwell", Price = 9.99m },
    new Book { Id = 2, Title = "Brave New World", Author = "Aldous Huxley", Price = 8.49m },
    new Book { Id = 3, Title = "Fahrenheit 451", Author = "Ray Bradbury", Price = 7.50m },
    new Book { Id = 4, Title = "The Hobbit", Author = "J.R.R. Tolkien", Price = 10.99m },
    new Book { Id = 5, Title = "To Kill a Mockingbird", Author = "Harper Lee", Price = 6.95m }
};
foreach (var book in books)
{
    Console.WriteLine($"Id: {book.Id}, Title: {book.Title}, Author: {book.Author}, Price: {book.Price}");
}

app.MapGet("/books", (string? author, string? title, decimal? minPrice, decimal? maxPrice) =>
{
    var filteredBooks = books.AsQueryable();
    if (!string.IsNullOrEmpty(author))
        {
            filteredBooks = filteredBooks.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
        }
    if (!string.IsNullOrEmpty(title))
        {
            filteredBooks = filteredBooks.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }
    if (minPrice.HasValue)
        {
            filteredBooks = filteredBooks.Where(b => b.Price >= minPrice.Value);
        }
    if (maxPrice.HasValue)
        {
            filteredBooks = filteredBooks.Where(b => b.Price <= maxPrice.Value);
        }
    return filteredBooks.ToList();
});

app.MapGet("/books/{id}", (int id) =>
{
    var book = books.FirstOrDefault(b => b.Id == id);
    if (book == null)
    {
        return Results.NotFound($"Book with id {id} not found.");
    }
    return Results.Ok(book);
});

app.MapPost("/books", (Book newBook) =>
{
    int newId;
    if (books.Any())
    {
        newId = books.Max(b => b.Id) + 1;
    }else
    {
        newId = 1;
    }
    newBook.Id = newId;
    books.Add(newBook);
    return Results.Created($"/books/{newId}", newBook);
});

app.MapPut("/books/{id}", (int id, Book updatedBook) =>
{
    var book = books.FirstOrDefault(b => b.Id == id);
    if (book == null)
    {
        return Results.NotFound($"Book with id {id} was not found.");
    }
    if (!string.IsNullOrEmpty(updatedBook.Title))
    {
        book.Title = updatedBook.Title;
    }
    if (!string.IsNullOrEmpty(updatedBook.Author))
    {
        book.Author = updatedBook.Author;
    }
    if(updatedBook.Price > 0)
    {
        book.Price = updatedBook.Price;
    }
    return Results.Ok(book);
}).RequireAuthorization();

app.MapDelete("/books/{id}", (int id) =>
{
    var book = books.FirstOrDefault(b => b.Id == id);
    if (book == null)
    {
        return Results.NotFound($"Book with id {id} not found.");
    }

    books.Remove(book);
    return Results.Ok($"Book with id {id} has been deleted.");
}).RequireAuthorization();

app.Run();
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;    
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
}