using System.Data;
using Chirp.Core.DTOs;
using Chirp.Core.Entities;
using Chirp.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;

namespace Chirp.Infrastructure.Services;

/// <summary>
/// Service for managing author relationships
/// </summary>
public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    /// <summary>
    /// Initializes a new instance of the AuthorService with the given repository.
    /// </summary>
    /// <param name="authorRepository">The repository for author operations.</param>
    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    /// <summary>
    /// Adds a follow relationship between two authors.
    /// </summary>
    /// <param name="user">The username of the follower.</param>
    /// <param name="toFollow">The username of the followee.</param>
    public async Task Follow(string user, string toFollow)
    {
        await _authorRepository.Follow(user, toFollow);
    }

    /// <summary>
    /// Removes a follow relationship between two authors.
    /// </summary>
    /// <param name="user">The username of the follower.</param>
    /// <param name="toUnfollow">The username of the followee.</param>
    public async Task Unfollow(string user, string toUnfollow)
    {
        await _authorRepository.Unfollow(user, toUnfollow);
    }

    /// <summary>
    /// Retrieves a list of usernames that an author follows.
    /// </summary>
    /// <param name="userId">The ID of the author.</param>
    /// <returns>A list of usernames the author follows.</returns>
    public async Task<List<string>> GetFollowedUsers(string userId)
    {
        return await _authorRepository.GetFollowedUsers(userId);
    }

    /// <summary>
    /// Checks if one author follows another.
    /// </summary>
    /// <param name="user">The username of the follower.</param>
    /// <param name="following">The username of the followee.</param>
    /// <returns>True if the follower follows the followee, otherwise false.</returns>
    public async Task<bool> Follows(string user, string following)
    {
        return await _authorRepository.Follows(user, following);
    }
}