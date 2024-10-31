using Microsoft.EntityFrameworkCore;
using Chirp.Core.Entities;
namespace Chirp.Infrastructure.Data
{

    public class ChirpDBContext : DbContext
    {

        public DbSet<Author> Authors { get; set; }

        public DbSet<Cheep> Cheeps { get; set; }


        public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
        {


        }

    }

}