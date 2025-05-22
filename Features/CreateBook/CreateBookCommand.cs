using MediatR;
using Bookstore.Api.Models;

namespace Bookstore.Api.Features.CreateBook
{
    public record CreateBookCommand(string Title, string Author, decimal Price) : IRequest<Book>;
}
