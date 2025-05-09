using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagement.Controllers
{
    [Route("api/v1/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDetailDto>>> GetBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<BookDetailDto>>> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            return Ok(book);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDetailDto>> PostBook(BookDto bookDto)
        {
            try
            {
                var book = await _bookService.AddBookAsync(bookDto);
                return CreatedAtAction(nameof(GetBook), new { id = book.Categories }, book);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutBook(int id, BookDto bookDto)
        {
            try
            {
                await _bookService.UpdateBookAsync(id, bookDto);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _bookService.DeleteBookAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("get-paged-books")]
        public async Task<ActionResult<IEnumerable<BookDetailDto>>> GetPagedBooks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            var books = await _bookService.GetPagedBooksAsync(pageNumber, pageSize);
            return Ok(books);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDetailDto>>> SearchBooks(
            [FromQuery] string keyword,
            [FromQuery] int lastId = 0,
            [FromQuery] int pageSize = 10)
        {
            var books = await _bookService.SearchBooksPagedAsync(keyword, lastId, pageSize);
            return Ok(books);
        }
    }
}
