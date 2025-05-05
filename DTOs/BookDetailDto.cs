namespace LibraryManagement.DTOs
{
    public class BookDetailDto : BookDto
    {
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }
}
