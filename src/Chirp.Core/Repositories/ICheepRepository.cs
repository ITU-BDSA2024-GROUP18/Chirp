using Chirp.Core.DTOs;
using Chirp.Core.Entities;

namespace Chirp.Core.Repositories;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadPublicTimeline(int pageNum);
    public Task<List<CheepDTO>> ReadFromAuthor(int pageNum, string author);

    public Task<Author> GetAuthorByName(string name);

    public Task<Author> GetAuthorByEmail(string email);

    public Task AddAuthor(Author author);

    public Task AddCheep(Cheep cheep);
    public Task<string> GetLatestIdAuthor();

    public Task<int> GetLatestIdCheep();

    public Task<Author?> CheckAuthorExists(string AuthorId);

    public Task<bool> Follows(string user, string following);

    public Task Follow(string user, string toFollow);

}