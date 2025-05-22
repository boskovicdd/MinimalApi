using MediatR;
using Bookstore.Api.Models;
namespace Bookstore.Api.Features.GetBookById
{
    public record GetBookByIdQuery(int Id) : IRequest<Book?>;
}
