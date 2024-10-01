using System.Data;
using Microsoft.Data.Sqlite;

//always define namespace everywhere except for Program.cs
namespace Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);


}

public class CheepService : ICheepService
{


    

    private DbFacade db = new DbFacade();

    public List<CheepViewModel> GetCheeps()
    {

        // Establish connection
        var connection = db.DBConnectionManager();

        // Build query without author param
        var query_string = @$"SELECT u.username, m.text, m.pub_date 
                              FROM message m 
                              JOIN user u ON m.author_id = u.user_id
                              ORDER BY m.pub_date DESC";


        var Cheep_list = db.ReadCheepsFromQuery(connection, query_string);

        return Cheep_list;

    }

    public List<CheepViewModel> GetCheepsFromAuthor(string authorQuery)
    {
        // Establish connection
        var connection = db.DBConnectionManager();

        // Build query with author paramater
        var query_string = @$"SELECT u.username, m.text, m.pub_date 
                              FROM message m 
                              JOIN user u ON m.author_id = u.user_id
                              WHERE u.username = '{authorQuery}'
                              ORDER BY m.pub_date DESC";

        // Retreive all cheeps from a single user database
        var Cheep_list = db.ReadCheepsFromQuery(connection, query_string);

        return Cheep_list;

    }


    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
