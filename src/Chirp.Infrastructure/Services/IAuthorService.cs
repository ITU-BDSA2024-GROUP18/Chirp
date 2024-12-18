using Chirp.Core.Entities;
using Chirp.Core.DTOs;

namespace Chirp.Infrastructure.Services;

/// <summary>
/// Interface defining services for managing author relationships
/// </summary>
public interface IAuthorService
{
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

    /// <summary>
    /// Checks if one author follows another.
    /// </summary>
    /// <param name="user">The username of the follower.</param>
    /// <param name="following">The username of the followee.</param>
    /// <returns>True if the follower follows the followee, otherwise false.</returns>
    public Task<bool> Follows(string user, string following);
}