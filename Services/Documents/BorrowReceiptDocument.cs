using LibraryManagement.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LibraryManagement.Services.Documents
{
    public class BorrowReceiptDocument : IDocument
    {
        private readonly List<BorrowedBook> _borrowedBooks;

        public BorrowReceiptDocument(List<BorrowedBook> borrowedBooks)
        {
            _borrowedBooks = borrowedBooks;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var user = _borrowedBooks.First().User;
            var borrowDate = _borrowedBooks.First().BorrowDate;
            var dueDate = _borrowedBooks.First().DueDate;

            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Text("HÓA ĐƠN MƯỢN SÁCH")
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                page.Content().Column(col =>
                {
                    col.Item().Text($"Họ tên: {user.Username}");
                    col.Item().Text($"Email: {user.Email}");
                    col.Item().Text($"Ngày mượn: {borrowDate:dd/MM/yyyy}");
                    col.Item().Text($"Hạn trả: {dueDate:dd/MM/yyyy}");
                    col.Item().Text($"Trạng thái: {_borrowedBooks.First().Status}");
                    col.Item().PaddingVertical(10);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Title
                            columns.RelativeColumn(2); // Author
                            columns.RelativeColumn(2); // ISBN
                        });

                        // Header row
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Tên sách").Bold();
                            header.Cell().Element(CellStyle).Text("Tác giả").Bold();
                            header.Cell().Element(CellStyle).Text("ISBN").Bold();

                            IContainer CellStyle(IContainer container) =>
                                container.DefaultTextStyle(x => x.SemiBold()).Padding(5).Background(Colors.Grey.Lighten2).Border(1);
                        });

                        foreach (var item in _borrowedBooks)
                        {
                            table.Cell().Element(CellStyle).Text(item.Book.Title);
                            table.Cell().Element(CellStyle).Text(item.Book.Author);
                            table.Cell().Element(CellStyle).Text(item.Book.ISBN);

                            IContainer CellStyle(IContainer container) =>
                                container.Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                        }
                    });

                    col.Item().PaddingTop(10).Text($"Tổng số sách: {_borrowedBooks.Count}");
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Thư viện ABC - ").SemiBold();
                    x.Span($"Ngày in: {DateTime.Now:dd/MM/yyyy}");
                });
            });
        }
    }
}