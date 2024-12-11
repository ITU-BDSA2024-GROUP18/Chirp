using Chirp.Core.DTOs;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Repositories;

public interface IAuthorRepository
{
    public Task<Author> GetAuthorByName(string name);
    public Task<Author> GetAuthorByEmail(string email);
    public Task AddAuthor(Author author);
    public Task<Author?> CheckAuthorExists(string authorId);
    public Task<string> GetLatestIdAuthor();
    public Task<bool> Follows(string user, string following);
    public Task Follow(string user, string toFollow);
    public Task Unfollow(string user, string toUnfollow);
    public Task<List<string>> GetFollowedUsers(string userId);
}