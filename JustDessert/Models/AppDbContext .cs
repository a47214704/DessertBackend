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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Product>(entity =>
			{
				entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
				entity.Property(e =>e.Name).HasMaxLength(50);
				entity.Property(e => e.Description).HasMaxLength(50);
				entity.Property(e => e.Status);
				entity.HasKey(e => e.Inventory);
			});
		}
	}
}
