using Chirp.Core.Entities;
using Chirp.Core.DTOs;
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
    public async Task<AuthorDTO> GetAuthorByName(string name)
    {
        var query =
            from author in _dbContext.Authors
            where author.UserName == name
            select author;

        var result = await query.FirstOrDefaultAsync();

        return new AuthorDTO
        {
            Id = result?.Id!,
            Username = result?.UserName!,
        } ?? throw new Exception("Author does not exist.");
    }

    public async Task<Author?> CheckAuthorExists(string authorId)
    {
        var query = _dbContext.Authors.Where(a => a.Id == authorId);
        return await query.FirstOrDefaultAsync();
    }

    // Commands


    public async Task<bool> Follows(string user, string following)
    {
        AuthorDTO authordto = await GetAuthorByName(user);
        AuthorDTO authorfolloweddto = await GetAuthorByName(following);

        Author author = _dbContext.Authors.First(auth => auth.UserName == authordto.Username);
        Author authorfollowed = _dbContext.Authors.First(auth => auth.UserName == authorfolloweddto.Username);

        if (author.Follows == null)
        {
            author.Follows = [];
        }

        if (author.Follows.Contains(authorfollowed))
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
        AuthorDTO authordto = await GetAuthorByName(user);
        AuthorDTO tofollowdto = await GetAuthorByName(toFollow);

        Author author = _dbContext.Authors.First(auth => auth.UserName == authordto.Username);
        Author tofollow = _dbContext.Authors.First(auth => auth.UserName == tofollowdto.Username);

        if (author.Follows == null)
        {
            author.Follows = [];
        }

        author.Follows.Add(tofollow);

        await _dbContext.SaveChangesAsync();
    }

    public async Task Unfollow(string user, string toUnfollow)
    {
        AuthorDTO authordto = await GetAuthorByName(user);
        AuthorDTO tounfollowdto = await GetAuthorByName(toUnfollow);

        //Linq here also?

        Author author = _dbContext.Authors.First(auth => auth.UserName == authordto.Username);
        Author tounfollow = _dbContext.Authors.First(auth => auth.UserName == tounfollowdto.Username);

        //And perhaps here?

        author.Follows?.Remove(tounfollow);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<string>> GetFollowedUsers(string userId)
    {
        var author = await _dbContext.Authors.Include(a => a.Follows).FirstOrDefaultAsync(a => a.Id == userId);
        return author!.Follows.Select(f => f.UserName!).ToList() ?? [];
    }
}