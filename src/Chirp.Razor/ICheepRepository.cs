namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadPublicTimeline(int pageNum);
    public Task<List<CheepDTO>> ReadFromAuthor(int pageNum, string author);

    public Task<Author> GetAuthor(string name);


}