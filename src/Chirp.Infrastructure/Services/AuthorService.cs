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

    public async Task<AuthorDTO> CreateAuthor(string name, string email)
    {
        // Fetch the latest ID as a string
        var latestIdString = await _authorRepository.GetLatestIdAuthor();

        // Parse the ID to an integer 
        var newId = (int.Parse(latestIdString) + 1).ToString();

        var author = new AuthorDTO()
        {
            Id = newId, // Assign the new incremented ID as a string
            Username = name,
        };
        return author;
    }

    public async Task AddAuthor(Author author)
    {
        await _authorRepository.AddAuthor(author);
    }
}