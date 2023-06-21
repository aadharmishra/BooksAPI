using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BooksAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            var books = _bookRepository.GetAllBooks();
            if (books == null)
                return NotFound();

            return Ok(books);
        }

        [HttpGet("{id}")]
        public IActionResult GetBookById(int id)
        {
            var book = _bookRepository.GetBookById(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpPost]
        public IActionResult AddBook([FromBody] Book book, [FromQuery] int ttlSeconds = 0)
        {
            if (book == null)
                return BadRequest();

            if(ttlSeconds==0)
            {
                _bookRepository.AddBook(book);
                return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
            }
            else
            {
                TimeSpan ttl = TimeSpan.FromSeconds(ttlSeconds);
                _bookRepository.AddBookWithTTL(book, ttl);
                return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
            }           
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] Book book)
        {
            if (book == null || book.Id != id)
                return BadRequest();

            var existingBook = _bookRepository.GetBookById(id);
            if (existingBook == null)
                return NotFound();

            _bookRepository.UpdateBook(book, existingBook);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var existingBook = _bookRepository.GetBookById(id);
            if (existingBook == null)
                return NotFound();

            _bookRepository.DeleteBook(id);
            return NoContent();
        }

        [HttpGet("author/{author}")]
        public IActionResult GetBooksByAuthor(string author)
        {
            var books = _bookRepository.GetBooksByAuthor(author);
            return Ok(books);
        }

        [HttpGet("title/{title}")]
        public IActionResult GetBooksByTitle(string title)
        {
            var books = _bookRepository.GetBooksByTitle(title);
            return Ok(books);
        }
    }
}
