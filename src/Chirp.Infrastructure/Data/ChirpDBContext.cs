using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Entities;
namespace Chirp.Infrastructure.Data
{

    public class ChirpDBContext : IdentityDbContext<Author, IdentityRole<int>, int>
    {

        public DbSet<Author> Authors { get; set; }

        public DbSet<Cheep> Cheeps { get; set; }


        public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
        {


        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Create a shadow property
            builder.Entity<Author>()
                .Property<string>("Name") // This is a shadow property
                .HasColumnName("UserName");  // Map it to the "UserName" column in the database
            
            builder.Entity<Author>()
                .Property<int>("AuthorId")
                .HasColumnName("Id");
        }
    }

}