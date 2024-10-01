using System.Data;
using Microsoft.Data.Sqlite;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);


}

public class CheepService : ICheepService
{

    private readonly string _connection_string = "/tmp/chirp.db";


    // These would normally be loaded from a database for example
    // private static readonly List<CheepViewModel> _cheeps = new()
    //     {
    //         new CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
    //         new CheepViewModel("Adrian", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
    //     };

    public List<CheepViewModel> GetCheeps()
    {
        string GetCheepsQuery = @"SELECT u.username, m.text, m.pub_date 
                                FROM message m 
                                JOIN user u ON m.author_id = u.user_id 
                                ORDER by m.pub_date desc";


        var cheeps = new List<CheepViewModel>();

        using (var connection = new SqliteConnection($"Data Source={_connection_string}"))
        {
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = GetCheepsQuery;

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var author = reader.GetString(0);
                var message = reader.GetString(1);
                var timeDouble = reader.GetDouble(2);
                string timestamp = UnixTimeStampToDateTimeString(timeDouble);

                var cheep = new CheepViewModel(author, message, timestamp);

                cheeps.Add(cheep);
            }

            return cheeps;

        }
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string authorQuery)
    {

        string GetCheepsFromAuthorQuery = @$"SELECT u.username, m.text, m.pub_date 
                                            FROM message m 
                                            JOIN user u ON m.author_id = u.user_id
                                            WHERE u.username = @AUTHOR
                                            ORDER by m.pub_date desc";

        var cheeps = new List<CheepViewModel>();

        using (var connection = new SqliteConnection($"Data Source={_connection_string}"))
        {
            connection.Open();

            //Create a SqliteCommand
            var command = connection.CreateCommand();

            //Set the SQL command to GetCheepsFromAuthorQuery string
            command.CommandText = GetCheepsFromAuthorQuery;

            //Add a varying parameter @AUTHOR in the collection of parameters used by the command
            command.Parameters.Add("@AUTHOR", SqliteType.Text);

            //Set the varying parameter to the authorQuery string that was passed to the function
            command.Parameters["@AUTHOR"].Value = authorQuery;

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var author = reader.GetString(0);
                var message = reader.GetString(1);
                var timeDouble = reader.GetDouble(2);
                string timestamp = UnixTimeStampToDateTimeString(timeDouble);

                var cheep = new CheepViewModel(author, message, timestamp);

                cheeps.Add(cheep);
            }


            return cheeps;
        }
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
