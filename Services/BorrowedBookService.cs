using AutoMapper;
using LibraryManagement.DTOs.Response;
using LibraryManagement.Enums;
using LibraryManagement.Models;
using LibraryManagement.Repositories;
using LibraryManagement.Services.Payments.Transactions;

namespace LibraryManagement.Services
{
    public class BorrowedBookService : IBorrowedBookService
    {
        private readonly IBorrowedBookRepository _borrowedBookRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;

        public BorrowedBookService(IBorrowedBookRepository borrowedBookRepository, IBookRepository bookRepository, IMapper mapper, ITransactionService transactionService)
        {
            _borrowedBookRepository = borrowedBookRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _transactionService = transactionService;
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

        public async Task<(bool Success, string Message)> BorrowBookAsync(int userId, List<int> bookIds)
        {
            if (bookIds == null || bookIds.Count == 0)
                return (false, "No books specified");

            decimal totalAmount = 0;
            var borrowedBooks = new List<BorrowedBook>();

            foreach (var bookId in bookIds)
            {
                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book == null)
                    return (false, $"Book with ID {bookId} not found");

                int currentlyBorrowed = book.BorrowedBooks.Count(bb => bb.Status != BorrowStatus.Returned.ToString());
                if (currentlyBorrowed >= book.Quantity)
                    return (false, $"No available copies for book {book.Title}");

                bool alreadyBorrowed = await _borrowedBookRepository.IsAlreadyBorrowedAndNotReturned(userId, bookId);
                if (alreadyBorrowed)
                    return (false, $"You already borrowed the book '{book.Title}' and have not returned it");

                var borrowed = new BorrowedBook
                {
                    BookId = bookId,
                    UserId = userId,
                    BorrowDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(14),
                    Status = BorrowStatus.Borrowed.ToString()
                };

                borrowedBooks.Add(borrowed);

                totalAmount += book.Price;
            }

            // Thêm các bản ghi mượn vào DB
            foreach (var borrowed in borrowedBooks)
            {
                await _borrowedBookRepository.AddAsync(borrowed);
                // Nếu muốn, giảm số lượng sách (nhưng lưu ý phần quantity như đã nói)
                var book = await _bookRepository.GetByIdAsync(borrowed.BookId);
                book.Quantity -= 1;
                await _bookRepository.UpdateAsync(book);
            }

            return (true, $"Books borrowed successfully.");
        }


        public async Task<(bool Success, string Message)> ReturnBookAsync(int userId, int bookId)
        {
            // Lấy thông tin mượn sách của người dùng
            var borrowedBook = await _borrowedBookRepository
                .GetByUserIdAndBookIdAsync(userId, bookId);

            if (borrowedBook == null)
                return (false, "You haven't borrowed this book.");

            if (borrowedBook.Status == BorrowStatus.Returned.ToString())
                return (false, "You have already returned this book.");

            // Cập nhật trạng thái thành "Returned" và lưu ngày trả sách
            borrowedBook.Status = BorrowStatus.Returned.ToString();
            borrowedBook.ReturnDate = DateTime.Now;

            await _borrowedBookRepository.UpdateAsync(borrowedBook);

            // Cập nhật số lượng sách có sẵn (nếu cần)
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book != null)
            {
                // Tăng số lượng sách có sẵn sau khi người dùng trả lại
                book.Quantity += 1;
                await _bookRepository.UpdateAsync(book);
            }

            return (true, "Book returned successfully.");
        }
    }
}
