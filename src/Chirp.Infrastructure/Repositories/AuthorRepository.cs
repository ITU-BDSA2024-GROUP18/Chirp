using Chirp.Core.Entities;
using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDBContext _dbContext;

    public AuthorRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
        dbContext.Database.EnsureCreated();
        DbInitializer.SeedDatabase(_dbContext);
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

    public async Task<List<string>> GetFollowedUsers(string userId)
    {
        var author = await _dbContext.Authors.Include(a => a.Follows).FirstOrDefaultAsync(a => a.Id == userId);
        return author!.Follows.Select(f => f.UserName!).ToList() ?? [];
    }
}