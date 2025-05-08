using BookCRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCRUD.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar las propiedades opcionales
            modelBuilder.Entity<Book>()
                .Property(b => b.FilePath)
                .IsRequired(false);

            modelBuilder.Entity<Book>()
                .Property(b => b.Content)
                .IsRequired(false);

            modelBuilder.Entity<Book>()
                .Property(b => b.CoverImageUrl)
                .IsRequired(false);
        }
    }
}