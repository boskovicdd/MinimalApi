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
        
        int newId;
        if (_books.Any())
        {
            newId = _books.Max(b => b.Id) + 1;
        }
        else
        {
            newId = 1;
        }
        var book = new Book
        {
            Id = newId,
            Title = request.Title,
            Author = request.Author,
            Price = request.Price
        };
        _books.Add(book);
        return Task.FromResult(book);

    }
}
