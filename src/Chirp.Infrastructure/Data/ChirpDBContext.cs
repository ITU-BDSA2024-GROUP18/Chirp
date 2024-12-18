using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Data
{
    /// <summary>
    /// Database context for the Chirp application, integrating identity and application entities.
    /// </summary>
    public class ChirpDBContext : IdentityDbContext<Author>
    {
        /// <summary>
        /// DbSet for managing Author entities.
        /// </summary>
        public DbSet<Author> Authors { get; set; }

        /// <summary>
        /// DbSet for managing Cheep entities.
        /// </summary>
        public DbSet<Cheep> Cheeps { get; set; }

        /// <summary>
        /// Initializes a new instance of the ChirpDBContext with specified options.
        /// </summary>
        /// <param name="options">Configuration options for the database context.</param>
        public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
        {
        }
    }
}