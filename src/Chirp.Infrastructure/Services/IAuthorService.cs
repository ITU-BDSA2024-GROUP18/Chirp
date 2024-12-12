using Chirp.Core.Entities;
using Chirp.Core.DTOs;

namespace Chirp.Infrastructure.Services;

public interface IAuthorService
{
    public Task<AuthorDTO> CreateAuthor(string name, string email);
    public Task AddAuthor(Author author);
}