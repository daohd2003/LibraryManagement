using LibraryManagement.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace LibraryManagement.Services.Documents
{
    public class ReceiptService
    {
        private readonly LibraryDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ReceiptService(LibraryDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<string> GenerateBorrowReceiptPdf(int userId, DateTime borrowDate)
        {
            var borrowedBooks = await _context.BorrowedBooks
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => b.UserId == userId && b.BorrowDate.Date == borrowDate.Date)
                .ToListAsync();

            if (borrowedBooks == null || borrowedBooks.Count == 0)
            {
                throw new Exception("Không tìm thấy dữ liệu mượn sách.");
            }

            var document = new BorrowReceiptDocument(borrowedBooks);

            // Thư mục lưu file
            string receiptsFolder = Path.Combine(_env.WebRootPath, "receipts/borrow");

            if (!Directory.Exists(receiptsFolder))
                Directory.CreateDirectory(receiptsFolder);

            string fileName = $"HoaDonMuonSach_{userId}_{borrowDate:yyyyMMdd}.pdf";
            string filePath = Path.Combine(receiptsFolder, fileName);

            document.GeneratePdf(filePath);

            return filePath;
        }

        public async Task<string> GenerateReturnReceiptPdf(int userId, DateTime returnDate)
        {
            var returnedBooks = await _context.BorrowedBooks
                .Include(b => b.Book)
                .Include(b => b.User)
                .Include(b => b.Penalties)
                .Where(b => b.UserId == userId && b.ReturnDate.Date == returnDate.Date)
                .ToListAsync();

            if (!returnedBooks.Any())
                throw new Exception("Không có sách nào được trả vào ngày này.");

            var document = new ReturnReceiptDocument(returnedBooks);

            // Thư mục lưu file
            string receiptsFolder = Path.Combine(_env.WebRootPath, "receipts/return");

            if (!Directory.Exists(receiptsFolder))
                Directory.CreateDirectory(receiptsFolder);

            string fileName = $"BienNhanTraSach_{userId}_{returnDate:yyyyMMdd}.pdf";
            string filePath = Path.Combine(receiptsFolder, fileName);

            document.GeneratePdf(filePath);

            return filePath;
        }
    }

}
