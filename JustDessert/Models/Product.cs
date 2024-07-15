using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JustDessert.Models
{
	public class Product
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public long Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(50)]
		public string? Description { get; set; }

		public int Status { get; set; } = (int)ProductStatus.Unknown;

		public long Inventory {  get; set; }
	}
}
