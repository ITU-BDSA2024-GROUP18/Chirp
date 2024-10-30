using Microsoft.EntityFrameworkCore;
using Chirp.Core.Entities;
namespace Chirp.Infrastructure.Data
{

    public class ChirpDBContext : DbContext
    {

        public required DbSet<Author> Authors { get; set; }

        public required DbSet<Cheep> Cheeps { get; set; }


        public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
        {


        }

    }

}