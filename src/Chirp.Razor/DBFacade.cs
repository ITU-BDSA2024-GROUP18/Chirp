using System.Data;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Http.Extensions;


namespace Chirp.Razor;


public class DbFacade
{

    private readonly string? _connection_string;

    //Embedded SQLite scripts, to be used if path from connection_string does not exist
    //private readonly IFileProvider embeddedFile;


    //Initialize DbFacade
    public DbFacade()
    {
        _connection_string = Environment.GetEnvironmentVariable("CHIRPDBPATH");

        //Consider maybe moving this to the DBConnectionManager function (AJ)
        if (_connection_string == null)
        {

            _connection_string = Path.Combine(AppContext.BaseDirectory, Path.GetTempPath() + "chirp.db");

        }

        seedDatabase();


    }

    public string readEmbeddedQuery(string queryPath)
    {


        var unEmbedder = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        using var streamEmbedded = unEmbedder.GetFileInfo(queryPath).CreateReadStream();
        using var streamReader = new StreamReader(streamEmbedded);

        var myQuery = streamReader.ReadToEnd();


        return myQuery;
    }

    public void seedDatabase()
    {

        var connection = DBConnectionManager();

        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText = readEmbeddedQuery("/data/schema.sql");

        command.ExecuteNonQuery();

        command.CommandText = readEmbeddedQuery("data/dump.sql");

        command.ExecuteNonQuery();



    }

    /*
    public string QueryBuilder(string? authorQuery)
    {


        //Base query for username, text, pub_date
        string SQLiteQuery = @$"SELECT u.username, m.text, m.pub_date 
                             FROM message m 
                             JOIN user u ON m.author_id = u.user_id";


        //If an author name (authorQuery) has been provided, append a WHERE clause to the query
        if (authorQuery != null) SQLiteQuery += $" WHERE u.username = '{authorQuery}'";

        //Order by date/time in a descending order
        SQLiteQuery += " ORDER BY m.pub_date DESC";

        return SQLiteQuery;


    }
    */

    public SqliteConnection DBConnectionManager()
    {

        //Makes a connection to the database tmp/chirp.db (temporary)
        var connection = new SqliteConnection($"Data Source={_connection_string}");


        //Returns the open connection (remember to close)
        return connection;
    }

    public List<CheepViewModel> ReadCheepsFromQuery(SqliteConnection connection, string SQLquery)
    {


        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText = SQLquery;

        using var reader = command.ExecuteReader();

        var cheeps = new List<CheepViewModel>();

        while (reader.Read())
        {
            var author = reader.GetString(0);
            var message = reader.GetString(1);
            var timeDouble = reader.GetDouble(2);
            string timestamp = UnixTimeStampToDateTimeString(timeDouble);

            var cheep = new CheepViewModel(author, message, timestamp);

            cheeps.Add(cheep);
        }

        connection.Close();

        return cheeps;

    }

    public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        dateTime = dateTime.AddSeconds(unixTimeStamp);

        //var local = dateTime.ToLocalTime();

        return dateTime.ToString("dd/MM/yy H:mm:ss");
    }

}



