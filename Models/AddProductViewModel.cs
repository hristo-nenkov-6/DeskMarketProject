using DeskMarket.Common;
using DeskMarket.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace DeskMarket.Models
{
    public class AddProductViewModel
    {
        [Required]
        [StringLength(ValidationConstants.ProductNameMaxLength,
            MinimumLength = ValidationConstants.ProductNameMinLength,
            ErrorMessage = $"Product name must be between 2 and 60")]
        public string ProductName { get; set; } = null!;

        [Required]
        [Range(typeof(Decimal), ValidationConstants.PriceMin, ValidationConstants.PriceMax,
            ErrorMessage = "Price must be between 1.00 and 3000.00")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(ValidationConstants.ProductDescriptionMaxLength,
            MinimumLength = ValidationConstants.ProductDescriptionMinLength,
            ErrorMessage = $"Product name must be between 10 and 250")]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; } = null!;

        [Required]
        public string AddedOn { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }

        public ICollection<GetCategoriesViewModel> Categories { get; set; } 
            = new List<GetCategoriesViewModel>();
    }
}
