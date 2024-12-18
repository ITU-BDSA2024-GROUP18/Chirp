using Chirp.Core.DTOs;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Repositories;

/// <summary>
/// Interface defining operations for managing Cheeps
/// </summary>
public interface ICheepRepository
{
    /// <summary>
    /// Retrieves a paginated list of Cheeps for the public timeline.
    /// </summary>
    /// <param name="pageNum">The page number to retrieve.</param>
    /// <returns>A list of CheepDTOs representing the public timeline.</returns>
    public Task<List<CheepDTO>> ReadPublicTimeline(int pageNum);

    /// <summary>
    /// Retrieves a paginated list of Cheeps from an author's follows and their own posts.
    /// </summary>
    /// <param name="pageNum">The page number to retrieve.</param>
    /// <param name="author">The username of the author.</param>
    /// <returns>A list of CheepDTOs.</returns>
    public Task<List<CheepDTO>> ReadFromFollows(int pageNum, string author);

    /// <summary>
    /// Adds a new Cheep to the database.
    /// </summary>
    /// <param name="cheep">The Cheep entity to add.</param>
    public Task AddCheep(Cheep cheep);

    /// <summary>
    /// Retrieves the ID of the most recent Cheep.
    /// </summary>
    /// <returns>The ID of the latest Cheep.</returns>
    public Task<int> GetLatestIdCheep();

    /// <summary>
    /// Deletes a specific Cheep based on author ID, timestamp, and message.
    /// </summary>
    /// <param name="authorid">The ID of the author.</param>
    /// <param name="timestamp">The timestamp of the Cheep.</param>
    /// <param name="message">The message of the Cheep.</param>
    public Task DeleteCheep(string? authorid, string timestamp, string message);
}