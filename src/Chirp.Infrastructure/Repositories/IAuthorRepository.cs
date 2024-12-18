using Chirp.Core.DTOs;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Repositories;

/// <summary>
/// Interface defining operations for managing authors
/// </summary>
public interface IAuthorRepository
{
    /// <summary>
    /// Retrieves an author's details by username.
    /// </summary>
    /// <param name="name">The username of the author.</param>
    /// <returns>An AuthorDTO containing the author's details.</returns>
    public Task<AuthorDTO> GetAuthorByName(string name);

    /// <summary>
    /// Checks if an author exists by their ID.
    /// </summary>
    /// <param name="authorId">The ID of the author.</param>
    /// <returns>The Author if found, otherwise null.</returns>
    public Task<Author?> CheckAuthorExists(string authorId);

    /// <summary>
    /// Determines if one author follows another.
    /// </summary>
    /// <param name="user">The username of the follower.</param>
    /// <param name="following">The username of the followee.</param>
    /// <returns>True if the follower follows the followee, otherwise false.</returns>
    public Task<bool> Follows(string user, string following);

    /// <summary>
    /// Adds a follow relationship between two authors.
    /// </summary>
    /// <param name="user">The username of the follower.</param>
    /// <param name="toFollow">The username of the followee.</param>
    public Task Follow(string user, string toFollow);

    /// <summary>
    /// Removes a follow relationship between two authors.
    /// </summary>
    /// <param name="user">The username of the follower.</param>
    /// <param name="toUnfollow">The username of the followee.</param>
    public Task Unfollow(string user, string toUnfollow);

    /// <summary>
    /// Retrieves a list of usernames that an author follows.
    /// </summary>
    /// <param name="userId">The ID of the author.</param>
    /// <returns>A list of usernames the author follows.</returns>
    public Task<List<string>> GetFollowedUsers(string userId);
}