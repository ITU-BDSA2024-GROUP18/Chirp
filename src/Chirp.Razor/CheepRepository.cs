namespace Chirp.Razor;


public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task GetCheeps(int pagenum)
    {

        return Task.CompletedTask;
    }

    public async Task GetCheepsFromAuthor(int pagenum, string author)
    {

        return Task.CompletedTask;
    }


}