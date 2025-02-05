using DeskMarket.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeskMarket.Data.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.ProductNameMaxLength,
            MinimumLength = ValidationConstants.ProductNameMinLength)]
        public string ProductName { get; set; } = null!;

        [Required]
        [StringLength(ValidationConstants.ProductDescriptionMaxLength,
            MinimumLength = ValidationConstants.ProductDescriptionMinLength)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(typeof(Decimal), ValidationConstants.PriceMin, ValidationConstants.PriceMax)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? ImageUrl {  get; set; }

        [Required]
        public string SellerId { get; set; } = null!;

        [Required]
        public IdentityUser Seller { get; set; } = null!;

        [Required]
        [DisplayFormat(DataFormatString = ValidationConstants.DateTimeFormat,
            ApplyFormatInEditMode = true)]
        public DateTime AddedOn { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;

        public ICollection<ProductClient> ProductsClients =
            new List<ProductClient>();
    }
}
