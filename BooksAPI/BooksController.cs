using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult AddBook([FromBody] Book book)
        {
            if (book == null)
                return BadRequest();

            _bookRepository.AddBook(book);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] Book book)
        {
            if (book == null || book.Id != id)
                return BadRequest();

            var existingBook = _bookRepository.GetBookById(id);
            if (existingBook == null)
                return NotFound();

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            _bookRepository.UpdateBook(existingBook);

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
    }
}
