using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Bookstore.Api.Models;
using Bookstore.Api.Features.CreateBook;
using Bookstore.Api.Features.GetBooks;
using Bookstore.Api.Features.GetBookById;
using Bookstore.Api.Features.DeleteBook;
using Bookstore.Api.Features.UpdateBook;
var books = new List<Book>
{
    new Book { Id = 1, Title = "1984", Author = "George Orwell", Price = 9.99m },
    new Book { Id = 2, Title = "Brave New World", Author = "Aldous Huxley", Price = 8.49m },
    new Book { Id = 3, Title = "Fahrenheit 451", Author = "Ray Bradbury", Price = 7.50m },
    new Book { Id = 4, Title = "The Hobbit", Author = "J.R.R. Tolkien", Price = 10.99m },
    new Book { Id = 5, Title = "To Kill a Mockingbird", Author = "Harper Lee", Price = 6.95m }
};

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});
builder.Services.AddSingleton<List<Book>>(books);
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(Program).Assembly);
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
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
       claims: claims,
       expires: DateTime.UtcNow.AddMinutes(60),
       signingCredentials: creds
   );
    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = tokenString });
});



foreach (var book in books)
{
    Console.WriteLine($"Id: {book.Id}, Title: {book.Title}, Author: {book.Author}, Price: {book.Price}");
}

app.MapGet("/books", async (string? author, string? title, decimal? minPrice, decimal? maxPrice, IMediator mediator) =>
{
    var query = new GetBooksQuery(author, title, minPrice, maxPrice);
    var books = await mediator.Send(query);
    return Results.Ok(books);
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

app.MapPost("/books", async (CreateBookCommand command, IMediator mediator) =>
{
    if (string.IsNullOrWhiteSpace(command.Title))
    {
        return Results.BadRequest("Book must have a title");
     }
    else if (command.Title.Length < 3)
    {
        return Results.BadRequest("Title must be longer than 3 characaters");
    }

    if (string.IsNullOrWhiteSpace(command.Author))
    {
        return Results.BadRequest("Author is required");
    }
    else if (command.Title.Length < 3)
    {
        return Results.BadRequest("Author must be longer than 3 characaters");
    }

    if (command.Price <= 0)
    {
        return Results.BadRequest("Price must be greater than zero");
    }
        
    var createdBook = await mediator.Send(command);
    return Results.Created($"/books/{createdBook.Id}", createdBook);
    
   

});

app.MapPut("/books/{id}", async (int id, UpdateBookCommand command, IMediator mediator) =>
{
    if (string.IsNullOrWhiteSpace(command.Title))
    {
        return Results.BadRequest("Book must have a title");
    }
    else if (command.Title.Length < 3)
    {
        return Results.BadRequest("Title must be longer than 3 characaters");
    }

    if (string.IsNullOrWhiteSpace(command.Author))
    {
        return Results.BadRequest("Author is required");
    }
    else if (command.Title.Length < 3)
    {
        return Results.BadRequest("Author must be longer than 3 characaters");
    }

    if (command.Price <= 0)
    {
        return Results.BadRequest("Price must be greater than zero");
    }
    
    var fullCommand = new FullUpdateBookCommand(id, command.Title, command.Author, command.Price);
    var updatedBook = await mediator.Send(fullCommand);
    if (updatedBook == null)
    {
        return Results.NotFound("Book not found");
    }
    return Results.Ok(updatedBook);
}).RequireAuthorization();

app.MapDelete("/books/{id}", async (int id, IMediator mediator) =>
{
    bool result = await mediator.Send(new DeleteBookCommand(id));
    if (result == false)
    {
        return Results.NotFound($"Book with id {id} was not found");
    }
    return Results.Ok($"Book with id {id} was deleted.");

}).RequireAuthorization();

app.Run();