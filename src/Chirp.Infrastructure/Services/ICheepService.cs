using Chirp.Core.DTOs;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Services;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int pageNum);
    public Task<List<CheepDTO>> GetCheepsFromFollows(int pageNum, string author);
    public Task AddCheep(Cheep cheep);
    public Task<Cheep> CreateCheep(string authorname, string message);
    public Task DeleteCheep(string? authorid, string timestamp, string message);
}