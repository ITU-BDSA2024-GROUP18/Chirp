using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Entities;
namespace Chirp.Infrastructure.Data
{

    public class ChirpDBContext : IdentityDbContext<Author>
    {

        public required DbSet<Author> Authors { get; set; }

        public required DbSet<Cheep> Cheeps { get; set; }

        public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
        {


        }


    }

}