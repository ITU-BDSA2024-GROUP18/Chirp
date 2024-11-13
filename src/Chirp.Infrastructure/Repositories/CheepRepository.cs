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
            select new CheepDTO
            {
                AuthorName = cheeps.Author.UserName,
                Message = cheeps.Text,
                Timestamp = cheeps.TimeStamp.ToString()
            };

        return await query.Skip((pagenum - 1) * 32).Take(32).ToListAsync();
    }

    public async Task<List<CheepDTO>> ReadFromAuthor(int pagenum, string author)
    {
        var query =
            from cheeps in _dbContext.Cheeps
            where cheeps.Author.UserName == author
            orderby cheeps.TimeStamp descending
            select new CheepDTO
            {
                AuthorName = cheeps.Author.UserName,
                Message = cheeps.Text,
                Timestamp = cheeps.TimeStamp.ToString()
            };

        return await query.Skip((pagenum - 1) * 32).Take(32).ToListAsync();
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
        var query = _dbContext.Authors.OrderByDescending(a => a.Id).Select(a => a.Id);
        return await query.FirstOrDefaultAsync();
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

