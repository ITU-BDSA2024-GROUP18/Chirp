using System.Data;
using Chirp.Core.DTOs;
using Chirp.Core.Entities;
using Chirp.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;

namespace Chirp.Infrastructure.Services;

/// <summary>
/// Service for managing Cheeps
/// </summary>
public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;

    /// <summary>
    /// Initializes a new instance of the CheepService with the given repositories.
    /// </summary>
    /// <param name="cheepRepository">The repository for Cheep operations.</param>
    /// <param name="authorRepository">The repository for Author operations.</param>
    public CheepService(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    /// <summary>
    /// Retrieves a paginated list of Cheeps for the public timeline.
    /// </summary>
    /// <param name="pageNum">The page number to retrieve.</param>
    /// <returns>A list of CheepDTOs.</returns>
    public async Task<List<CheepDTO>> GetCheeps(int pageNum)
    {
        return await _cheepRepository.ReadPublicTimeline(pageNum);
    }

    /// <summary>
    /// Creates a new Cheep for an existing author or initializes a new author before creating the Cheep.
    /// </summary>
    /// <param name="authorid">The ID of the author.</param>
    /// <param name="message">The content of the Cheep.</param>
    /// <returns>The created Cheep entity.</returns>
    public async Task<Cheep> CreateCheep(string authorid, string message)
    {
        var author = await _authorRepository.CheckAuthorExists(authorid);

        var cheep = new Cheep
        {
            CheepId = await _cheepRepository.GetLatestIdCheep() + 1,
            Author = author!,
            AuthorId = author!.Id,
            Text = message,
            TimeStamp = DateTime.Now
        };

        return cheep;
    }

    /// <summary>
    /// Adds a new Cheep to the database.
    /// </summary>
    /// <param name="cheep">The Cheep entity to add.</param>
    public async Task AddCheep(Cheep cheep)
    {
        await _cheepRepository.AddCheep(cheep);
    }

    /// <summary>
    /// Deletes a specific Cheep based on author ID, timestamp, and message.
    /// </summary>
    /// <param name="authorid">The ID of the author.</param>
    /// <param name="timestamp">The timestamp of the Cheep.</param>
    /// <param name="message">The content of the Cheep.</param>
    public async Task DeleteCheep(string? authorid, string timestamp, string message)
    {
        await _cheepRepository.DeleteCheep(authorid, timestamp, message);
    }

    /// <summary>
    /// Retrieves a paginated list of Cheeps from an author's follows and their own posts.
    /// </summary>
    /// <param name="pageNum">The page number to retrieve.</param>
    /// <param name="author">The username of the author.</param>
    /// <returns>A list of CheepDTOs.</returns>
    public async Task<List<CheepDTO>> GetCheepsFromFollows(int pageNum, string author)
    {
        return await _cheepRepository.ReadFromFollows(pageNum, author);
    }
}