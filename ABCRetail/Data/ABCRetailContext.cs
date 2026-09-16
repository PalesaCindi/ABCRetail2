using ABCRetail.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ABCRetail.Data
{
    public class ABCRetailContext : IdentityDbContext<ApplicationUser>
    {
        public ABCRetailContext(
            DbContextOptions<ABCRetailContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Product { get; set; }

        public DbSet<Customer> Customer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customer implements ITableEntity for Azure Table Storage;
            // these fields aren't relational-mappable, so EF Core must ignore them.
            modelBuilder.Entity<Customer>().Ignore(c => c.PartitionKey);
            modelBuilder.Entity<Customer>().Ignore(c => c.RowKey);
            modelBuilder.Entity<Customer>().Ignore(c => c.Timestamp);
            modelBuilder.Entity<Customer>().Ignore(c => c.ETag);

            // Product implements ITableEntity too, same reasoning.
            modelBuilder.Entity<Product>().Ignore(p => p.PartitionKey);
            modelBuilder.Entity<Product>().Ignore(p => p.RowKey);
            modelBuilder.Entity<Product>().Ignore(p => p.Timestamp);
            modelBuilder.Entity<Product>().Ignore(p => p.ETag);
        }
    }
}