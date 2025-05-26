using System;

namespace Bookstore.Api.Models
{
    public class Book
    {
        public int Id { get;  set; }
        public string Title { get;  set; }
        public string Author { get;  set; }
        public decimal Price { get;  set; }

        public Book(string title, string author, decimal price)
        {
            if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
                throw new ArgumentException("Title must be at least 3 characters long.");

            if (string.IsNullOrWhiteSpace(author) || author.Length < 3)
                throw new ArgumentException("Author must be at least 3 characters long.");

            if (price <= 0)
                throw new ArgumentException("Price must be greater than 0.");

            Title = title;
            Author = author;
            Price = price;
        }

        
        public Book() { }
    }
}
