using MediatR;
using Bookstore.Api.Models;
namespace Bookstore.Api.Features.UpdateBook
{
    public class UpdateTitleHandler : IRequestHandler<FullUpdateTitleCommand, Book?>
    {
        private readonly List<Book> _books;
        public UpdateTitleHandler(List<Book> books)
        {
            _books = books;
        }
        public Task<Book?> Handle(FullUpdateTitleCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.NewTitle) || request.NewTitle.Length < 3)
            {
                throw new ArgumentException("Title must be at least 3 characters long.");
            }
                
            var book = _books.FirstOrDefault(b => b.Id == request.BookId);
            if (book == null)
            {
                return Task.FromResult<Book?>(null);
            }
            book.Title = request.NewTitle;
            return Task.FromResult(book);
        }
    }
}