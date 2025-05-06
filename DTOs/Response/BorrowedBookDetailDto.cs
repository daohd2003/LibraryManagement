namespace LibraryManagement.DTOs.Response
{
    public class BorrowedBookDetailDto : BorrowedBookDto
    {
        public BookDto Book { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }
}
