using System.Data;
using Chirp.Core.DTOs;
using Chirp.Core.Entities;
using Chirp.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;

//always define namespace everywhere except for Program.cs
namespace Chirp.Infrastructure.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task Follow(string user, string toFollow)
    {
        await _authorRepository.Follow(user, toFollow);
    }

    public async Task Unfollow(string user, string toUnfollow)
    {
        await _authorRepository.Unfollow(user, toUnfollow);
    }

    public async Task<List<string>> GetFollowedUsers(string userId)
    {
        return await _authorRepository.GetFollowedUsers(userId);
    }

    public async Task<bool> Follows(string user, string following)
    {
        return await _authorRepository.Follows(user, following);
    }
}