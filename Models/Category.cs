namespace LibraryManagement.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;

        // Navigation properties
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}
