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
            var bookJson = _database.StringGet(GetRedisKey(id));
            return JsonConvert.DeserializeObject<Book>(bookJson);
        }

        public void AddBook(Book book)
        {
            var bookJson = JsonConvert.SerializeObject(book);
            _database.StringSet(GetRedisKey(book.Id), bookJson);
        }

        public void UpdateBook(Book book)
        {
            var existingBook = GetBookById(book.Id);
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

    }
}
