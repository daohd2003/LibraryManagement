namespace LibraryManagement.DTOs.Response
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public int Quantity { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
