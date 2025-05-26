using Bookstore.Api.Features.CreateBook;
using Bookstore.Api.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class CreateBookHandler : IRequestHandler<CreateBookCommand, Book>
{
    private readonly List<Book> _books;
    public CreateBookHandler(List<Book> books)
    {
        _books = books;
    }
    public Task<Book> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = new Book(request.Title, request.Author, request.Price);
        int newId;
        if (_books.Any())
        {
            newId = _books.Max(b => b.Id) + 1;
        }
        else
        {
            newId = 1;
        }
        book.Id = newId;
        _books.Add(book);
        return Task.FromResult(book);

    }
}
