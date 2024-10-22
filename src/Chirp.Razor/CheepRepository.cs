using System.Linq;
namespace Chirp.Razor;

public interface ICheepRepository
{
    public List<CheepViewModel> ReadPublicTimeline(int pageNum);
    public List<CheepViewModel> ReadPrivateTimeline(int pageNum, string author);
}

public class CheepRepository : ICheepRepository
{
    private int pageLimit = 32;

    private DbFacade db = new DbFacade();

	private ChirpDBContext _ChirpDBContext;

	public CheepRepository(ChirpDBContext context)
	{
		_ChirpDBContext = context;
	}

	//Use Task<List<>> here from now on
    public List<CheepViewModel> ReadPublicTimeline(int pageNum)
    {

        // Establish connection
        var connection = db.DBConnectionManager();

        // Build query without author param
        /*var query_string = @$"SELECT u.username, m.text, m.pub_date 
                              FROM message m 
                              JOIN user u ON m.author_id = u.user_id
                              ORDER BY m.pub_date DESC 
                              LIMIT {pageLimit} OFFSET {(pageNum - 1) * pageLimit}";*/

		var LINQ_query = (from cheep in _ChirpDBContext.Cheeps
						 orderby cheep.Timestamp descending
						 select new CheepViewModel ( cheep.Author.Name,
													 cheep.Text,
													 cheep.Timestamp.ToString("dd/MM/yy H:mm:ss")
												    )
						 ).Skip((pageNum - 1) * pageLimit).Take( pageLimit );

						 


        //If you wanna test pagination with different pageLimit values,
        //author Jacqualine Gilcoine has a total of 359 Cheeps on her timeline

        var Cheep_list = LINQ_query.ToList();

        return Cheep_list;

    }

    public List<CheepViewModel> ReadPrivateTimeline(int pageNum, string authorQuery)
    {
        // Establish connection
        var connection = db.DBConnectionManager();

        // Build query with author paramater
        /*var query_string = @$"SELECT u.username, m.text, m.pub_date 
                              FROM message m 
                              JOIN user u ON m.author_id = u.user_id
                              WHERE u.username = '{authorQuery}'
                              ORDER BY m.pub_date DESC
                              LIMIT {pageLimit} OFFSET {(pageNum - 1) * pageLimit}";*/

        var LINQ_query = (from cheep in _ChirpDBContext.Cheeps
						 where cheep.Author.Name == authorQuery
						 orderby cheep.Timestamp descending
						 select new CheepViewModel ( cheep.Author.Name,
													 cheep.Text,
													 cheep.Timestamp.ToString("dd/MM/yy H:mm:ss")
												    )
						 ).Skip((pageNum - 1) * pageLimit).Take( pageLimit );
        
        // Retreive all cheeps from a single user database
        var Cheep_list = LINQ_query.ToList();

        return Cheep_list;

    }
    
}