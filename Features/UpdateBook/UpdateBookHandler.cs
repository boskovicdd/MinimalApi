using System;
using MediatR;
using Bookstore.Api.Models;
namespace Bookstore.Api.Features.UpdateBook
{
    public class UpdateBookHandler : IRequestHandler<FullUpdateBookCommand, Book?>
    {
        public readonly List<Book> _books;
        public UpdateBookHandler(List<Book> books)
        {
            _books = books;
        }
        public Task<Book?> Handle(FullUpdateBookCommand request, CancellationToken cancellationToken)
        {
            
            var book = _books.FirstOrDefault(b => b.Id == request.Id);
            if (book == null)
            {
                return Task.FromResult<Book?>(null);
            }
            if (!string.IsNullOrEmpty(request.Title))
            {
                book.Title = request.Title;
            }
            if (!string.IsNullOrEmpty(request.Author))
            {
                book.Author = request.Author;
            }
            if (request.Price > 0)
            {
                book.Price = request.Price.Value;
            }
            return Task.FromResult<Book?>(book);
        }
    }
}
