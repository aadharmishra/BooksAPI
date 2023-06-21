using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BooksAPI
{
    public class RedisBookRepository: IBookRepository
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _database;

        public RedisBookRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _database = _connectionMultiplexer.GetDatabase();
        }

        private RedisKey GetRedisKey(int id) => $"books:{id}";

        public IEnumerable<Book> GetAllBooks()
        {
            var redisKeys = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First())
                .Keys(database: _database.Database, pattern: "books:*");
            var books = _database.StringGet(redisKeys.ToArray()).Select(entry => JsonConvert.DeserializeObject<Book>(entry));
            return books;
        }

        public Book GetBookById(int id)
        {
            var key = GetRedisKey(id);
            var bookJson = _database.StringGet(key);
            var ttl = _database.KeyTimeToLive(key);

            if (!bookJson.IsNull)
            {
                return JsonConvert.DeserializeObject<Book>(bookJson);
            }
            return null;
        }

        public void AddBook(Book book)
        {
            var bookJson = JsonConvert.SerializeObject(book);
            _database.StringSet(GetRedisKey(book.Id), bookJson);
        }

        public void AddBookWithTTL(Book book, TimeSpan ttl)
        {
            var bookJson = JsonConvert.SerializeObject(book);
            _database.StringSet(GetRedisKey(book.Id), bookJson, ttl);
        }

        public void UpdateBook(Book book, Book existingBook)
        {
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;

                var bookJson = JsonConvert.SerializeObject(existingBook);
                _database.StringSet(GetRedisKey(existingBook.Id), bookJson);
            }
        }

        public void DeleteBook(int id)
        {
            _database.KeyDelete(GetRedisKey(id));
        }

        public List<Book> GetBooksByAuthor(string author)
        {
            var redisKeys = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First())
                .Keys(database: _database.Database, pattern: "books:*");

            var books = _database.StringGet(redisKeys.ToArray())
                .Select(entry => JsonConvert.DeserializeObject<Book>(entry))
                .Where(book => book.Author.Equals(author, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return books;
        }

        public List<Book> GetBooksByTitle(string title)
        {
            var redisKeys = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First())
                .Keys(database: _database.Database, pattern: "books:*");

            var books = _database.StringGet(redisKeys.ToArray())
                .Select(entry => JsonConvert.DeserializeObject<Book>(entry))
                .Where(book => book.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return books;
        }

    }
}
