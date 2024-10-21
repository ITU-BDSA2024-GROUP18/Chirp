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

    public List<CheepViewModel> ReadPublicTimeline(int pageNum)
    {

        // Establish connection
        var connection = db.DBConnectionManager();

        // Build query without author param
        var query_string = @$"SELECT u.username, m.text, m.pub_date 
                              FROM message m 
                              JOIN user u ON m.author_id = u.user_id
                              ORDER BY m.pub_date DESC 
                              LIMIT {pageLimit} OFFSET {(pageNum - 1) * pageLimit}";


        //If you wanna test pagination with different pageLimit values,
        //author Jacqualine Gilcoine has a total of 359 Cheeps on her timeline

        var Cheep_list = db.ReadCheepsFromQuery(connection, query_string);

        return Cheep_list;

    }

    public List<CheepViewModel> ReadPrivateTimeline(int pageNum, string authorQuery)
    {
        // Establish connection
        var connection = db.DBConnectionManager();

        // Build query with author paramater
        var query_string = @$"SELECT u.username, m.text, m.pub_date 
                              FROM message m 
                              JOIN user u ON m.author_id = u.user_id
                              WHERE u.username = '{authorQuery}'
                              ORDER BY m.pub_date DESC
                              LIMIT {pageLimit} OFFSET {(pageNum - 1) * pageLimit}";

        // Retreive all cheeps from a single user database
        var Cheep_list = db.ReadCheepsFromQuery(connection, query_string);

        return Cheep_list;

    }
    
}