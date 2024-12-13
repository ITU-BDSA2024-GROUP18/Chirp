using Chirp.Core.DTOs;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Repositories;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadPublicTimeline(int pageNum);
    public Task<List<CheepDTO>> ReadFromAuthor(int pageNum, string author);
    public Task<List<CheepDTO>> ReadFromFollows(int pageNum, string author);
    public Task AddCheep(Cheep cheep);
    public Task<int> GetLatestIdCheep();
    public Task DeleteCheep(string? authorid, string timestamp, string message);
}