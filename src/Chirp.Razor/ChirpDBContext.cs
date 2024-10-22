using Microsoft.EntityFrameworkCore;
namespace Chirp.Razor
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