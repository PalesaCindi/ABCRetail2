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

        public DbSet<Order> Order { get; set; }
    }
}