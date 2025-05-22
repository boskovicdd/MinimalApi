using MediatR;
using System.Collections.Generic;
using Bookstore.Api.Models;
namespace Bookstore.Api.Features.GetBooks
{
    public record GetBooksQuery(string? Author, string? Title, decimal? MinPrice, decimal? MaxPrice) : IRequest<List<Book>>;


}
