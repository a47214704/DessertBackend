using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JustDessert.Models
{
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Name { get; set; } = string.Empty;

		[Required]
		[MaxLength(100)]
		public string Password { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public int Category {  get; set; }
	}
}
