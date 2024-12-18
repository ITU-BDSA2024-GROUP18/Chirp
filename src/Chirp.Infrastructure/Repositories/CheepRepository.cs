using System.Globalization;
using Chirp.Core.DTOs;
using Chirp.Core.Entities;
using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

/// <summary>
/// Repository for managing Cheep entities
/// </summary>
public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the CheepRepository with the given database context.
    /// Ensures the database is created and seeded.
    /// </summary>
    /// <param name="dbContext">The database context to use.</param>
    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
        dbContext.Database.EnsureCreated();
        DbInitializer.SeedDatabase(_dbContext);
    }

    /// <summary>
    /// Retrieves a paginated list of cheeps for the public timeline, ordered by timestamp.
    /// </summary>
    /// <param name="pagenum">The page number to retrieve.</param>
    /// <returns>A list of CheepDTOs.</returns>
    public async Task<List<CheepDTO>> ReadPublicTimeline(int pagenum)
    {
        var query = from cheeps in _dbContext.Cheeps
                    orderby cheeps.TimeStamp descending
                    select new { cheeps.Author, cheeps.Text, cheeps.TimeStamp };

        var result = await query.Skip((pagenum - 1) * 32).Take(32).ToListAsync();

        return result.Select(cheep => new CheepDTO
        {
            AuthorName = cheep.Author?.UserName ?? "Unknown Author",
            Message = cheep.Text,
            Timestamp = cheep.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
        }).ToList();
    }

    /// <summary>
    /// Retrieves a paginated list of cheeps from the author and authors they follow.
    /// </summary>
    /// <param name="pagenum">The page number to retrieve.</param>
    /// <param name="author">The username of the author.</param>
    /// <returns>A list of CheepDTOs.</returns>
    public async Task<List<CheepDTO>> ReadFromFollows(int pagenum, string author)
    {
        var query = from cheeps in _dbContext.Cheeps
                    where cheeps.Author.UserName == author ||
                          (from authors in _dbContext.Authors
                           where authors.UserName == author
                           from follow in authors.Follows
                           select follow).Contains(cheeps.Author)
                    orderby cheeps.TimeStamp descending
                    select new { cheeps.Author, cheeps.Text, cheeps.TimeStamp };

        var result = await query.Skip((pagenum - 1) * 32).Take(32).ToListAsync();

        return result.Select(cheep => new CheepDTO
        {
            AuthorName = cheep.Author?.UserName ?? "Unknown Author",
            Message = cheep.Text,
            Timestamp = cheep.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
        }).ToList();
    }

    /// <summary>
    /// Retrieves the ID of the most recent Cheep.
    /// </summary>
    /// <returns>The ID of the latest Cheep.</returns>
    public async Task<int> GetLatestIdCheep()
    {
        var query = _dbContext.Cheeps.OrderByDescending(c => c.CheepId).Select(c => c.CheepId);
        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Adds a new Cheep to the database.
    /// </summary>
    /// <param name="cheep">The Cheep to add.</param>
    public async Task AddCheep(Cheep cheep)
    {
        await _dbContext.Cheeps.AddAsync(cheep);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a specific Cheep based on author ID, timestamp, and message.
    /// </summary>
    /// <param name="authorid">The ID of the author.</param>
    /// <param name="timestamp">The timestamp of the Cheep.</param>
    /// <param name="message">The message of the Cheep.</param>
    public async Task DeleteCheep(string? authorid, string timestamp, string message)
    {
        var cheeps = _dbContext.Cheeps
            .Where(c => c.AuthorId == authorid && c.Text == message)
            .ToList();

        var cheepToDelete = cheeps.SingleOrDefault(c => c.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) == timestamp);

        if (cheepToDelete != null)
        {
            _dbContext.Cheeps.Remove(cheepToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}