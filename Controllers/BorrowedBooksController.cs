using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowedBooksController : ControllerBase
    {
        private readonly IBorrowedBookService _borrowedBookService;

        public BorrowedBooksController(IBorrowedBookService borrowedBookService)
        {
            _borrowedBookService = borrowedBookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowedBookDto>>> GetAll()
        {
            var books = await _borrowedBookService.GetAllAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowedBookDetailDto>> GetById(int id)
        {
            var book = await _borrowedBookService.GetByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BorrowedBookDto>> Create(BorrowedBook book)
        {
            var created = await _borrowedBookService.AddAsync(book);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, BorrowedBook book)
        {
            if (id != book.Id) return BadRequest();
            var result = await _borrowedBookService.UpdateAsync(book);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _borrowedBookService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("borrow")]
        public async Task<ActionResult> BorrowBook(int userId, int bookId)
        {
            var result = await _borrowedBookService.BorrowBookAsync(userId, bookId);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
