using System;
using System.Collections.Generic;

namespace BooksAPI
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAllBooks();
        Book GetBookById(int id);
        void AddBook(Book book);
        void AddBookWithTTL(Book book, TimeSpan ttl);
        void UpdateBook(Book book, Book existingBook);
        void DeleteBook(int id);
        List<Book> GetBooksByAuthor(string author);
        List<Book> GetBooksByTitle(string title);
    }
}
