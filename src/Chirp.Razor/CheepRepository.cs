using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;


public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
        // Ensure a database for context exists
        dbContext.Database.EnsureCreated();
        // Call script from Dbinitializer class - seeding our database
        DbInitializer.SeedDatabase(_dbContext);

    }


    public async Task<List<CheepDTO>> ReadPublicTimeline(int pagenum)
    {

        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query =
            from cheeps in _dbContext.Cheeps
            orderby cheeps.TimeStamp descending
            select new CheepDTO
            {
                AuthorName = cheeps.Author.Name,
                Message = cheeps.Text,
                Timestamp = UnixTimeStampToDateTimeString(
                        ((DateTimeOffset)cheeps.TimeStamp).ToUnixTimeSeconds()
                    )
            };

        // Execute the query and store the results
        var result = await query
                    .Skip((pagenum - 1) * 32)
                    .Take(32)
                    .ToListAsync();

        return result;

    }

    public async Task<List<CheepDTO>> ReadFromAuthor(int pagenum, string author)
    {

        var query =
           from cheeps in _dbContext.Cheeps
           where cheeps.Author.Name == author
           orderby cheeps.TimeStamp descending
           select new CheepDTO
           {
               AuthorName = cheeps.Author.Name,
               Message = cheeps.Text,
               Timestamp = UnixTimeStampToDateTimeString(
                        ((DateTimeOffset)cheeps.TimeStamp).ToUnixTimeSeconds()
                    )
           };

        // Execute the query and store the results
        var result = await query
                    .Skip((pagenum - 1) * 32)
                    .Take(32)
                    .ToListAsync();

        return result;



    }

    public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        dateTime = dateTime.AddSeconds(unixTimeStamp);

        //var local = dateTime.ToLocalTime();

        return dateTime.ToString("dd/MM/yy H:mm:ss");
    }


}