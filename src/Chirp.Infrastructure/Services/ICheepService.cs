using Chirp.Core.DTOs;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Services;

/// <summary>
/// Interface defining services for managing Cheeps
/// </summary>
public interface ICheepService
{
    /// <summary>
    /// Retrieves a paginated list of Cheeps for the public timeline.
    /// </summary>
    /// <param name="pageNum">The page number to retrieve.</param>
    /// <returns>A list of CheepDTOs.</returns>
    public Task<List<CheepDTO>> GetCheeps(int pageNum);

    /// <summary>
    /// Retrieves a paginated list of Cheeps from an author's follows and their own posts.
    /// </summary>
    /// <param name="pageNum">The page number to retrieve.</param>
    /// <param name="author">The username of the author.</param>
    /// <returns>A list of CheepDTOs.</returns>
    public Task<List<CheepDTO>> GetCheepsFromFollows(int pageNum, string author);

    /// <summary>
    /// Adds a new Cheep to the database.
    /// </summary>
    /// <param name="cheep">The Cheep entity to add.</param>
    public Task AddCheep(Cheep cheep);

    /// <summary>
    /// Creates a new Cheep with the specified author and message.
    /// </summary>
    /// <param name="authorname">The username of the author.</param>
    /// <param name="message">The content of the Cheep.</param>
    /// <returns>The created Cheep entity.</returns>
    public Task<Cheep> CreateCheep(string authorname, string message);

    /// <summary>
    /// Deletes a specific Cheep based on author ID, timestamp, and message.
    /// </summary>
    /// <param name="authorid">The ID of the author.</param>
    /// <param name="timestamp">The timestamp of the Cheep.</param>
    /// <param name="message">The content of the Cheep.</param>
    public Task DeleteCheep(string? authorid, string timestamp, string message);
}