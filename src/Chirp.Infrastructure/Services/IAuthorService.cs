using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Services;

public interface IAuthorService
{
    public Task<Author> CreateAuthor(string name, string email);
    public Task AddAuthor(Author author);
}