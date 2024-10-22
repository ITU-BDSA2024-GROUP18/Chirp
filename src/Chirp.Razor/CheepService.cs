using System.Data;
using Microsoft.Data.Sqlite;

//always define namespace everywhere except for Program.cs
namespace Chirp.Razor;

//ViewModel obsolete - only used in db.facade which our tests still uses.
public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int pageNum);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int pageNum, string author);


}

public class CheepService : ICheepService
{

    private readonly ICheepRepository _CheepRepository;

    // private int pageLimit = 32;

    // private DbFacade db = new DbFacade();

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

}
