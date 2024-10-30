namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadPublicTimeline(int pageNum);
    public Task<List<CheepDTO>> ReadFromAuthor(int pageNum, string author);

    public Task<Author> GetAuthorByName(string name);

    public Task<Author> GetAuthorByEmail(string email);

    public Task AddAuthor(Author author);

    public Task AddCheep(Cheep cheep);
    public Task<int> GetLatestIdAuthor();

    public Task<int> GetLatestIdCheep();

    public Task<Author> CheckAuthorExists(int AuthorId);


}