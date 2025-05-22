using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Bookstore.Api.Models;


namespace Bookstore.Api.Features.GetBookById
{
    public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, Book?>
    {
        private readonly List<Book> _books;
        public GetBookByIdHandler(List<Book> books)
        {
            _books = books;
        }
        public Task<Book?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = _books.FirstOrDefault(b => b.Id == request.Id);
            return Task.FromResult(book); 
        }
    }
}
