using MediatR;
using Bookstore.Api.Models;
namespace Bookstore.Api.Features.UpdateBook
{
    public class UpdateAuthorHandler : IRequestHandler<FullUpdateAuthorCommand, Book?>
    {
        private readonly List<Book> _books;
        public UpdateAuthorHandler(List<Book> books)
        {
            _books = books;
        }
        public Task<Book?> Handle(FullUpdateAuthorCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.NewAuthor) || request.NewAuthor.Length < 3)
            {
                throw new ArgumentException("Author name must be at least 3 characters long.");
            }
            var book = _books.FirstOrDefault(b => b.Id == request.BookId);
            if(book == null)
            {
                return Task.FromResult<Book?>(null);
            }
            book.Author = request.NewAuthor;
            return Task.FromResult(book);
        }
    }
}