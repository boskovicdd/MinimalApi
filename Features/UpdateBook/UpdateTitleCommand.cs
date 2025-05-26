using MediatR;
using Bookstore.Api.Models;
namespace Bookstore.Api.Features.UpdateBook
{
    public record FullUpdateTitleCommand(int BookId, string NewTitle) : IRequest<Book?>;
    public record UpdateTitleCommand(string NewTitle) : IRequest<Book?>;
}