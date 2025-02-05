using DeskMarket.Common;
using System.ComponentModel.DataAnnotations;

namespace DeskMarket.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.CategoryNameMaxLength, 
            MinimumLength = ValidationConstants.CategoryNameMinLength)]
        public string Name { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = 
            new List<Product>();
    }
}