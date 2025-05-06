using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs.Request
{
    public class UpdateCategoryDto : CreateCategoryDto
    {
        [Required]
        public int Id { get; set; }
    }
}
