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


    private readonly string _connection_string = "/tmp/chirp.db";

    private DbFacade db = new DbFacade();

    public List<CheepViewModel> GetCheeps()
    {

        // Establish connection
        var connection = db.DBConnectionManager();

        // Build query without author param
        var query_string = db.QueryBuilder(null);


        var Cheep_list = db.ReadCheepsFromQuery(connection, query_string);

        return Cheep_list;

    }

    public List<CheepViewModel> GetCheepsFromAuthor(string authorQuery)
    {
        // Establish connection
        var connection = db.DBConnectionManager();

        // Build query with author paramater
        var query_string = db.QueryBuilder(authorQuery);

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
