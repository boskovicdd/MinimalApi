using MediatR;

namespace Bookstore.Api.Features.DeleteBook
{
    public record DeleteBookCommand(int Id) : IRequest<bool>;
}
