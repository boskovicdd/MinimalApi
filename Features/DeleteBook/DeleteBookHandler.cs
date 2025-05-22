using System;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bookstore.Api.Models;

namespace Bookstore.Api.Features.DeleteBook
{
    public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, bool>
    {
        private readonly List<Book> _books;
        public DeleteBookHandler(List<Book> books)
        {
            _books = books;
        }
        public Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = _books.FirstOrDefault(b => b.Id == request.Id);
            if(book == null)
            {
                return Task.FromResult(false);
            }
            _books.Remove(book);
            return Task.FromResult(true);
            

        }
    }
}
