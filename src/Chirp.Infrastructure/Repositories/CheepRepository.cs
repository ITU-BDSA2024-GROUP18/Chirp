using System.Globalization;
using Chirp.Core.DTOs;
using Chirp.Core.Entities;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;

    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
        dbContext.Database.EnsureCreated();
        DbInitializer.SeedDatabase(_dbContext);
    }

    // Queries
    public async Task<List<CheepDTO>> ReadPublicTimeline(int pagenum)
    {
        var query =
            from cheeps in _dbContext.Cheeps
            orderby cheeps.TimeStamp descending
            select new
            {
                cheeps.Author,
                cheeps.Text,
                cheeps.TimeStamp
            };

        var result = await query.Skip((pagenum - 1) * 32).Take(32).ToListAsync();

        return result.Select(cheep => new CheepDTO
        {
            AuthorName = cheep.Author?.UserName ?? "Unknown Author", // Handle null here
            Message = cheep.Text,
            Timestamp = cheep.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
        }).ToList();
    }

    public async Task<List<CheepDTO>> ReadFromAuthor(int pagenum, string author)
    {
        var query =
            from cheeps in _dbContext.Cheeps
            where cheeps.Author != null && cheeps.Author.UserName == author
            orderby cheeps.TimeStamp descending
            select new
            {
                Author = cheeps.Author,
                Text = cheeps.Text,
                TimeStamp = cheeps.TimeStamp
            };

        var result = await query.Skip((pagenum - 1) * 32).Take(32).ToListAsync();

        return result.Select(cheep => new CheepDTO
        {
            AuthorName = cheep.Author?.UserName ?? "Unknown Author", // Handle null here
            Message = cheep.Text,
            Timestamp = cheep.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
        }).ToList();
    }

    public async Task<List<CheepDTO>> ReadFromFollows(int pagenum, string author)
    {

        var query =

            from cheeps in _dbContext.Cheeps
                //Include cheeps of the logged in author
            where cheeps.Author.UserName == author || (

            //Get the IDs of who the current author follows
            //and use that to include their cheeps
                from authors in _dbContext.Authors
                where authors.UserName == author
                from follow in authors.Follows
                select follow).Contains(cheeps.Author)
            orderby cheeps.TimeStamp descending
            select new
            {

                Author = cheeps.Author,
                Text = cheeps.Text,
                TimeStamp = cheeps.TimeStamp

            };

        var result = await query.Skip((pagenum - 1) * 32).Take(32).ToListAsync();

        return result.Select(cheep => new CheepDTO
        {
            AuthorName = cheep.Author?.UserName ?? "Unknown Author", // Handle null here
            Message = cheep.Text,
            Timestamp = cheep.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
        }).ToList();



    }


    public async Task<Author> GetAuthorByName(string name)
    {
        var query =
            from author in _dbContext.Authors
            where author.UserName == name
            select author;

        return await query.FirstOrDefaultAsync() ?? throw new InvalidOperationException();
    }

    public async Task<Author> GetAuthorByEmail(string email)
    {
        var query =
            from author in _dbContext.Authors
            where author.Email == email
            select author;

        return await query.FirstOrDefaultAsync() ?? throw new InvalidOperationException();
    }

    public async Task<string> GetLatestIdAuthor()
    {
        var authors = await _dbContext.Authors
            .ToListAsync(); // Get all authors 

        var latestId = authors
            .OrderByDescending(a => int.Parse(a.Id))
            .Select(a => a.Id)
            .FirstOrDefault();

        return latestId ?? throw new InvalidOperationException("No authors found in the database.");
    }

    public async Task<int> GetLatestIdCheep()
    {
        var query = _dbContext.Cheeps.OrderByDescending(c => c.CheepId).Select(c => c.CheepId);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<Author?> CheckAuthorExists(string authorId)
    {
        var query = _dbContext.Authors.Where(a => a.Id == authorId);
        return await query.FirstOrDefaultAsync();
    }

    // Commands
    public async Task AddAuthor(Author author)
    {
        await _dbContext.Authors.AddAsync(author);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCheep(Cheep cheep)
    {
        await _dbContext.Cheeps.AddAsync(cheep);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> Follows(string user, string following)
    {
        Author author = await GetAuthorByName(user);
        Author authorFollowed = await GetAuthorByName(following);

        if (author.Follows == null)
        {
            author.Follows = [];
        }


        /// In this method we would maybe use LINQ instead



        if (author.Follows.Contains(authorFollowed))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task Follow(string user, string toFollow)
    {
        Author author = await GetAuthorByName(user);
        Author authorToFollow = await GetAuthorByName(toFollow);

        if (author.Follows == null)
        {
            author.Follows = [];
        }

        //Linq here also?

        author.Follows.Add(authorToFollow);

        await _dbContext.SaveChangesAsync();
    }

    public async Task Unfollow(string user, string toUnfollow)
    {
        Author author = await GetAuthorByName(user);
        Author authorToUnfollow = await GetAuthorByName(toUnfollow);

        //And perhaps here?

        author.Follows?.Remove(authorToUnfollow);

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCheeps(string? authorid, string timestamp, string message)
    {

        //Fetch all cheeps from an author that has a match between DTO message and cheep text
        var cheeps = _dbContext.Cheeps
            .Where(c => c.AuthorId == authorid && c.Text == message)
            .ToList();


        var cheepToDelete = cheeps.SingleOrDefault(c => c.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) == timestamp);

        Console.WriteLine($"Database: {cheepToDelete?.TimeStamp}");
        Console.WriteLine($"Input: {timestamp}");
        if (cheepToDelete != null)
        {

            _dbContext.Cheeps.Remove(cheepToDelete);
            await _dbContext.SaveChangesAsync();

        }
    }




    // Helper method
    // public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    // {
    //     // Unix timestamp is seconds past epoch
    //     DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    //     dateTime = dateTime.AddSeconds(unixTimeStamp);

    //     //var local = dateTime.ToLocalTime();

    //     return dateTime.ToString("dd/MM/yy H:mm:ss", CultureInfo.InvariantCulture);
    // }


}