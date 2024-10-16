using Microsoft.EntityFrameworkCore;
namespace Chirp.Razor
{

    public class ChirpDBContext : DbContext
    {

        public required DbSet<Author> Authors;

        public required DbSet<Cheep> cheeps;


        public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
        {


        }

    }

}