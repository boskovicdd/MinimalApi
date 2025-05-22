using MediatR;
using Bookstore.Api.Models;
namespace Bookstore.Api.Features.UpdateBook
{
    public record UpdateBookCommand(string Title, string Author, decimal Price) : IRequest<Book?>;
    public record FullUpdateBookCommand(int Id,string? Title,string? Author,decimal? Price) : IRequest<Book?>;
}
