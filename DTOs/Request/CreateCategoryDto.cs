using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs.Request
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
