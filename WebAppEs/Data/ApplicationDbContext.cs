using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAppEs.Entity;
using WebAppEs.Models;

namespace WebAppEs.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
		{

		}

		public DbSet<RoleMenuPermission> RoleMenuPermission { get; set; }
		public DbSet<NavigationMenu> NavigationMenu { get; set; }
		public DbSet<MobileRND_Models> MobileRND_Models { get; set; }
		public DbSet<MobileRND_StoreHead> MobileRND_StoreHead { get; set; }
		public DbSet<MobileRND_StoreDetails> MobileRND_StoreDetails { get; set; }
		public DbSet<MobileRND_Store_ScanHistory> MobileRND_Store_ScanHistory { get; set; }
		

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<ApplicationUser>()
				.Property(e => e.Department)
				.HasMaxLength(100);

			builder.Entity<ApplicationUser>()
				.Property(e => e.Name)
				.HasMaxLength(100);

			builder.Entity<ApplicationUser>()
				.Property(e => e.EmployeeID)
				.HasMaxLength(100);

			builder.Entity<RoleMenuPermission>()
			.HasKey(c => new { c.RoleId, c.NavigationMenuId});

			base.OnModelCreating(builder);
		}
	}
}