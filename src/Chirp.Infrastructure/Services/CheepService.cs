using System.Data;
using Chirp.Core.DTOs;
using Chirp.Core.Entities;
using Chirp.Core.Repositories;
using Microsoft.Data.Sqlite;

//always define namespace everywhere except for Program.cs
namespace Chirp.Infrastructure.Services;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int pageNum);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNum, string author);

    public Task<Author> CreateAuthor(string name, string email);

    public Task AddAuthor(Author author);

    public Task AddCheep(Cheep cheep);

    public Task<Cheep> CreateCheep(string authorid, string message, string? authorname = null, string? email = null);




}

public class CheepService : ICheepService
{

    private readonly ICheepRepository _CheepRepository;


    public CheepService(ICheepRepository CheepRepository)
    {

        _CheepRepository = CheepRepository;

    }

    public async Task<List<CheepDTO>> GetCheeps(int pageNum)
    {

        var cheep_list = await _CheepRepository.ReadPublicTimeline(pageNum);

        return cheep_list;


    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNum, string author)
    {

        var cheep_list = await _CheepRepository.ReadFromAuthor(pageNum, author);

        return cheep_list;

    }

    public async Task<Author> CreateAuthor(string name, string email)
    {
        // Fetch the latest ID as a string
        var latestIdString = await _CheepRepository.GetLatestIdAuthor();

        // Parse the ID to an integer 
        var newId = (int.Parse(latestIdString) + 1).ToString();

        var author = new Author()
        {
            Id = newId, // Assign the new incremented ID as a string
            UserName = name,
            Email = email,
            Cheeps = new List<Cheep>()
        };
        return author;
    }


    public async Task<Cheep> CreateCheep(string authorid, string message, string? authorname = null, string? email = null)
    {
        //Checks if author exists in db based on an id, 
        //if the author does exists a new cheep is created for that author, if not a new author is created, before creating the cheep.

        var CheckAuthor = await _CheepRepository.CheckAuthorExists(authorid);

        if (CheckAuthor != null)
        {
            var cheep = new Cheep()
            {
                CheepId = await _CheepRepository.GetLatestIdCheep() + 1,
                Author = CheckAuthor,
                AuthorId = CheckAuthor.Id,
                Text = message,
                TimeStamp = DateTime.Now
            };

            return cheep;

        }
        else
        {
            //creating new author
            var newAuthor = await CreateAuthor(authorname ?? "Default Author", email ?? "default@example.com");

            var cheep = new Cheep()
            {
                CheepId = await _CheepRepository.GetLatestIdCheep() + 1,
                Author = newAuthor,
                AuthorId = newAuthor.Id,
                Text = message,
                TimeStamp = DateTime.Now
            };

            return cheep;

        }

    }
    public async Task AddAuthor(Author author)
    {

        await _CheepRepository.AddAuthor(author);

    }

    public async Task AddCheep(Cheep cheep)
    {

        await _CheepRepository.AddCheep(cheep);
    }

}
