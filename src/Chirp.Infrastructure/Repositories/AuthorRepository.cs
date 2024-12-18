using Chirp.Core.Entities;
using Chirp.Core.DTOs;
using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

/// <summary>
/// Repository for managing Author entities
/// </summary>
public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDBContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the AuthorRepository with the given database context.
    /// Ensures the database is created and seeded.
    /// </summary>
    /// <param name="dbContext">The database context to use.</param>
    public AuthorRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
        dbContext.Database.EnsureCreated();
        DbInitializer.SeedDatabase(_dbContext);
    }

    /// <summary>
    /// Retrieves an Author by username.
    /// </summary>
    /// <param name="name">The username to search for.</param>
    /// <returns>The matching AuthorDTO, or an exception if not found.</returns>
    public async Task<AuthorDTO> GetAuthorByName(string name)
    {
        var query = from author in _dbContext.Authors
                    where author.UserName == name
                    select author;

        var result = await query.FirstOrDefaultAsync();

        return new AuthorDTO
        {
            Id = result?.Id!,
            Username = result?.UserName!,
        } ?? throw new Exception("Author does not exist.");
    }

    /// <summary>
    /// Checks if an Author exists by their ID.
    /// </summary>
    /// <param name="authorId">The ID of the Author to check.</param>
    /// <returns>The matching Author or null if not found.</returns>
    public async Task<Author?> CheckAuthorExists(string authorId)
    {
        var query = _dbContext.Authors.Where(a => a.Id == authorId);
        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Checks if one Author follows another.
    /// </summary>
    /// <param name="user">The username of the follower.</param>
    /// <param name="following">The username of the followee.</param>
    /// <returns>True if the follower follows the followee; otherwise false.</returns>
    public async Task<bool> Follows(string user, string following)
    {
        AuthorDTO authordto = await GetAuthorByName(user);
        AuthorDTO authorfolloweddto = await GetAuthorByName(following);

        Author author = _dbContext.Authors.First(auth => auth.UserName == authordto.Username);
        Author authorfollowed = _dbContext.Authors.First(auth => auth.UserName == authorfolloweddto.Username);

        if (author.Follows == null)
        {
            author.Follows = new List<Author>();
        }

        return author.Follows.Contains(authorfollowed);
    }

    /// <summary>
    /// Adds a follow relationship between two Authors.
    /// </summary>
    /// <param name="user">The username of the follower.</param>
    /// <param name="toFollow">The username of the followee.</param>
    public async Task Follow(string user, string toFollow)
    {
        AuthorDTO authordto = await GetAuthorByName(user);
        AuthorDTO tofollowdto = await GetAuthorByName(toFollow);

        Author author = _dbContext.Authors.First(auth => auth.UserName == authordto.Username);
        Author tofollow = _dbContext.Authors.First(auth => auth.UserName == tofollowdto.Username);

        if (author.Follows == null)
        {
            author.Follows = new List<Author>();
        }

        author.Follows.Add(tofollow);

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Removes a follow relationship between two Authors.
    /// </summary>
    /// <param name="user">The username of the follower.</param>
    /// <param name="toUnfollow">The username of the followee.</param>
    public async Task Unfollow(string user, string toUnfollow)
    {
        AuthorDTO authordto = await GetAuthorByName(user);
        AuthorDTO tounfollowdto = await GetAuthorByName(toUnfollow);

        Author author = _dbContext.Authors.First(auth => auth.UserName == authordto.Username);
        Author tounfollow = _dbContext.Authors.First(auth => auth.UserName == tounfollowdto.Username);

        author.Follows?.Remove(tounfollow);

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves a list of usernames that an Author follows.
    /// </summary>
    /// <param name="userId">The ID of the Author.</param>
    /// <returns>A list of usernames the Author follows.</returns>
    public async Task<List<string>> GetFollowedUsers(string userId)
    {
        var author = await _dbContext.Authors.Include(a => a.Follows).FirstOrDefaultAsync(a => a.Id == userId);
        return author!.Follows.Select(f => f.UserName!).ToList() ?? new List<string>();
    }
}