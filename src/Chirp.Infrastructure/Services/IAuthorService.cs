using Chirp.Core.Entities;
using Chirp.Core.DTOs;

namespace Chirp.Infrastructure.Services;

public interface IAuthorService
{
    public Task<AuthorDTO> CreateAuthor(string name, string email);
    public Task AddAuthor(Author author);
    public Task Follow(string user, string toFollow);
    public Task Unfollow(string user, string toUnfollow);
    public Task<List<string>> GetFollowedUsers(string userId);
    public Task<bool> Follows(string user, string following);


}