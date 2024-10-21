namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetCheeps(int pageNum);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNum, string author);


}