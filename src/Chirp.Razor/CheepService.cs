using System.Data;
using Microsoft.Data.Sqlite;

//always define namespace everywhere except for Program.cs
namespace Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int pageNum);
    public List<CheepViewModel> GetCheepsFromAuthor(int pageNum, string author);


}

public class CheepService : ICheepService
{
	private ICheepRepository _CheepRepository;

    private int pageLimit = 32;

    private DbFacade db = new DbFacade();

	public CheepService(ICheepRepository repository)
	{
		_CheepRepository = repository;
	}

    public List<CheepViewModel> GetCheeps(int pageNum)
{
	var Cheep_List = _CheepRepository.ReadPublicTimeline(pageNum);

	return Cheep_List;
}

    public List<CheepViewModel> GetCheepsFromAuthor(int pageNum, string author)
{
	var Cheep_List = _CheepRepository.ReadPrivateTimeline(pageNum, author);

	return Cheep_List;
}

}
