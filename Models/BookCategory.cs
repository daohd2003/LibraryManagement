using System.Text.Json.Serialization;

namespace LibraryManagement.Models
{
    public class BookCategory
    {
        public int BookId { get; set; }
        public int CategoryId { get; set; }

        [JsonIgnore]
        public virtual Book Book {get; set;} = null!;
        [JsonIgnore]
        public virtual Category Category { get; set; } = null!;
    }
}
