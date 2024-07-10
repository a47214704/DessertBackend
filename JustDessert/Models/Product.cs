using System.ComponentModel.DataAnnotations.Schema;

namespace JustDessert.Models
{
	public class Product
	{
		[Column("id")]
		public long Id { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("description")]
		public string Description { get; set; }

		[Column("status")]
		public string Status { get; set; }

		[Column("inventory")]
		public long Inventory {  get; set; }
	}
}
