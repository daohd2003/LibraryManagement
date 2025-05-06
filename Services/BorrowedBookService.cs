using AutoMapper;
using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;
using LibraryManagement.Repositories;

namespace LibraryManagement.Services
{
    public class BorrowedBookService : IBorrowedBookService
    {
        private readonly IBorrowedBookRepository _borrowedBookRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BorrowedBookService(IBorrowedBookRepository borrowedBookRepository, IBookRepository bookRepository, IMapper mapper)
        {
            _borrowedBookRepository = borrowedBookRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BorrowedBookDto>> GetAllAsync()
        {
            var dto = await _borrowedBookRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<BorrowedBookDto>>(dto);
        }

        public async Task<BorrowedBookDetailDto?> GetByIdAsync(int id)
        {
            var dto = await _borrowedBookRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<BorrowedBookDetailDto?>(dto);
        }

        public async Task<BorrowedBookDto> AddAsync(BorrowedBook book)
        {
            var dto = await _borrowedBookRepository.AddAsync(book);
            return _mapper.Map<BorrowedBookDto>(dto);
        }

        public async Task<bool> UpdateAsync(BorrowedBook book)
        {
            return await _borrowedBookRepository.UpdateAsync(book);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _borrowedBookRepository.DeleteAsync(id);
        }

        public async Task<(bool Success, string Message)> BorrowBookAsync(int userId, int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return (false, "Book not found");

            // Đếm số sách đang được mượn mà chưa trả
            int currentlyBorrowed = book.BorrowedBooks
                .Count(bb => bb.Status != "Returned");

            if (currentlyBorrowed >= book.Quantity)
                return (false, "No available copies");

            // Kiểm tra người dùng đã mượn mà chưa trả chưa
            var alreadyBorrowed = await _borrowedBookRepository.IsAlreadyBorrowedAndNotReturned(userId, bookId);
            if (alreadyBorrowed)
                return (false, "You already borrowed this book and have not returned it");

            // Tạo bản ghi mượn sách
            var borrowed = new BorrowedBook
            {
                BookId = bookId,
                UserId = userId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                Status = "Borrowed"
            };

            await _borrowedBookRepository.AddAsync(borrowed);
            return (true, "Book borrowed successfully");
        }
    }
}
