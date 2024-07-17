using Microsoft.EntityFrameworkCore;

namespace JustDessert.Models
{
	public class AppDbContext : DbContext
	{
		public AppDbContext()
		{
		}

		public AppDbContext(DbContextOptions<AppDbContext> options)
			:base(options)
		{
		}

		public DbSet<Product> Products { get; set; }

		public DbSet<User> Users { get; set; }
	}
}
