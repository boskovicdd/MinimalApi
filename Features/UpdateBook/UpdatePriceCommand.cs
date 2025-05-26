using MediatR;
using Bookstore.Api.Models;
namespace Bookstore.Api.Features.UpdateBook
{
    public record UpdatePriceCommand(decimal NewPrice) : IRequest<Book?>;
    public record FullUpdatePriceCommand(int BookId, decimal NewPrice) : IRequest<Book?>;
}