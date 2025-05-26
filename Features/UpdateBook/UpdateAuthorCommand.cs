using MediatR;
using Bookstore.Api.Models;
namespace Bookstore.Api.Features.UpdateBook
{
    public record FullUpdateAuthorCommand(int BookId, string NewAuthor) : IRequest<Book?>;
    public record UpdateAuthorCommand(string NewAuthor) : IRequest<Book?>;
}