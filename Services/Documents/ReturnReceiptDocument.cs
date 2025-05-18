using LibraryManagement.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LibraryManagement.Services.Documents
{
    public class ReturnReceiptDocument : IDocument
    {
        private readonly List<BorrowedBook> _returnedBooks;

        public ReturnReceiptDocument(List<BorrowedBook> returnedBooks)
        {
            _returnedBooks = returnedBooks;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var user = _returnedBooks.First().User;
            var returnDate = _returnedBooks.First().ReturnDate;

            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Text("BIÊN NHẬN TRẢ SÁCH").FontSize(20).Bold().AlignCenter();

                page.Content().Column(col =>
                {
                    col.Item().Text($"Họ tên: {user.Username}");
                    col.Item().Text($"Email: {user.Email}");
                    col.Item().Text($"Ngày trả: {returnDate:dd/MM/yyyy}");
                    col.Item().Text($"Tổng sách trả: {_returnedBooks.Count}");
                    col.Item().PaddingVertical(10);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Title
                            columns.RelativeColumn(2); // Author
                            columns.RelativeColumn(2); // ISBN
                            columns.RelativeColumn(2); // Status
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Tên sách").Bold();
                            header.Cell().Text("Tác giả").Bold();
                            header.Cell().Text("ISBN").Bold();
                            header.Cell().Text("Trạng thái").Bold();
                        });

                        foreach (var item in _returnedBooks)
                        {
                            table.Cell().Text(item.Book.Title);
                            table.Cell().Text(item.Book.Author);
                            table.Cell().Text(item.Book.ISBN);
                            table.Cell().Text(item.Status);
                        }
                    });

                    // Phần tiền phạt
                    var penalties = _returnedBooks.SelectMany(b => b.Penalties).ToList();

                    if (penalties.Any())
                    {
                        col.Item().PaddingTop(20).Text("Tiền phạt").FontSize(14).Bold();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Vi phạm
                                columns.RelativeColumn(5); // Mô tả
                                columns.RelativeColumn(2); // Số tiền
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Vi phạm").Bold();
                                header.Cell().Text("Mô tả").Bold();
                                header.Cell().Text("Số tiền").Bold();
                            });

                            foreach (var p in penalties)
                            {
                                table.Cell().Text(p.ViolationType);
                                table.Cell().Text(p.Description);
                                table.Cell().Text($"{p.Amount:C0}"); // VNĐ style
                            }

                            decimal total = penalties.Sum(p => p.Amount);
                            table.Cell().ColumnSpan(2).AlignRight().Text("Tổng tiền phạt:");
                            table.Cell().Text($"{total:C0}");
                        });
                    }
                    else
                    {
                        col.Item().PaddingTop(20).Text("Không có tiền phạt.").Italic();
                    }
                });

                page.Footer().AlignCenter().Text($"Thư viện ABC - Ngày in: {DateTime.Now:dd/MM/yyyy}");
            });
        }
    }
}
