using System.Data;
using Chirp.Core.DTOs;
using Chirp.Core.Entities;
using Chirp.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Chirp.Infrastructure.Services;

//always define namespace everywhere except for Program.cs
namespace Chirp.Infrastructure.Services;

public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;

    public CheepService(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    public async Task<List<CheepDTO>> GetCheeps(int pageNum)
    {

        var cheep_list = await _cheepRepository.ReadPublicTimeline(pageNum);

        return cheep_list;


    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNum, string author)
    {
        var cheep_list = await _cheepRepository.ReadFromAuthor(pageNum, author);
        return cheep_list;
    }

    public async Task<Cheep> CreateCheep(string authorid, string message)
    {
        //Checks if author exists in db based on an id, 
        //if the author does exists a new cheep is created for that author, if not a new author is created, before creating the cheep.

        var author = await _authorRepository.CheckAuthorExists(authorid);

        var cheep = new Cheep()
        {
            CheepId = await _cheepRepository.GetLatestIdCheep() + 1,
            Author = author!,
            AuthorId = author!.Id,
            Text = message,
            TimeStamp = DateTime.Now
        };

        return cheep;
    }

    public async Task AddCheep(Cheep cheep)
    {
        await _cheepRepository.AddCheep(cheep);
    }

    public async Task DeleteCheep(string? authorid, string timestamp, string message)
    {
        await _cheepRepository.DeleteCheep(authorid, timestamp, message);
    }

    public async Task<List<CheepDTO>> GetCheepsFromFollows(int pageNum, string author)
    {
        return await _cheepRepository.ReadFromFollows(pageNum, author);
    }
}
