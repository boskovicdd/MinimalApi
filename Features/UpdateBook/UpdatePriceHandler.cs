using System;
using MediatR;
using Bookstore.Api.Models;

namespace Bookstore.Api.Features.UpdateBook
{
    public class UpdatePriceHandler : IRequestHandler<FullUpdatePriceCommand, Book?>
    {
        private readonly List<Book> _books;
        public UpdatePriceHandler(List<Book> books)
        {
            _books = books;
        }
        public Task<Book?> Handle(FullUpdatePriceCommand request, CancellationToken cancellationToken)
        {
            if (request.NewPrice <=0)
            {
                throw new ArgumentException("Price of the book must be greater than zero");
            }
            var book = _books.FirstOrDefault(b => b.Id == request.BookId);
            if (book == null)
            {
                return Task.FromResult<Book?>(null);
            }
            book.Price = request.NewPrice;
            return Task.FromResult(book);
        }
    }
}
