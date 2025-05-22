using System;
using MediatR;
using Bookstore.Api.Models;

namespace Bookstore.Api.Features.GetBooks
{
    public class GetBooksHandler : IRequestHandler<GetBooksQuery, List<Book>>
    {
        private readonly List<Book> _books;
        public GetBooksHandler(List<Book> books)
        {
            _books = books;
        }
        public Task<List<Book>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
        {
            var filteredBooks = _books.AsQueryable();
            if (!string.IsNullOrEmpty(request.Author))
            {
                filteredBooks = filteredBooks.Where(b => b.Author.Contains(request.Author, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(request.Title))
            {
                filteredBooks = filteredBooks.Where(b => b.Title.Contains(request.Title, StringComparison.OrdinalIgnoreCase));
            }
            if (request.MinPrice.HasValue)
            {
                filteredBooks = filteredBooks.Where(b => b.Price >= request.MinPrice.Value);
            }
            if (request.MaxPrice.HasValue)
            {
                filteredBooks = filteredBooks.Where(b => b.Price <= request.MaxPrice.Value);
            }
            return Task.FromResult(filteredBooks.ToList());
        }
    }
}
