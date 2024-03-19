using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection.Emit;

namespace Project.Models
{
    public class BookStoreContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options)
        {
        }

        public BookStoreContext() : base()
        {

        }

        DbSet<Book> Books { get; set; }
        DbSet<Author> Authors { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderDetails> OrdersDetails { get; set; }
        DbSet<Discount> Discounts { get; set; }
        DbSet<ApplicationUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<OrderDetails>().HasKey("Order_id", "Book_id");
            builder.Entity<Comment>().HasKey("user_id", "book_id");

            base.OnModelCreating(builder);
        }

    }
}
