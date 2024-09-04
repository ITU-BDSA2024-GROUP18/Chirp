using System.Globalization;
using CsvHelper;
using SimpleDB;

string timeFormat = "MM/dd/yy HH:mm:ss";

if (args.Length < 1) 
{  
    Console.WriteLine("How to use the program: \n" + 
                    " dotnet run -- <read> (Displays all cheeps in DB) \n" + 
                    " dotnet run -- <cheep> [message] (Cheep a message to the DB)");
}

if (args[0] == "read")
{
    Read();
} 
else if (args[0]  == "cheep")
{
    if (args.Length < 2)
    {
        Console.WriteLine("You are missing a message to cheep :(");
    }
    Cheep(args[1]);
}



void Read()
{
    using (var reader = new StreamReader("chirp_cli_db.csv"))
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        csv.GetRecords<Cheep>();     
        //For printing to terminal:
        foreach (var record in csv.GetRecords<Cheep>())
                {
                    Console.WriteLine($"{record.Author} @ {FromUnixTimeToDateTime(record.Timestamp)} : {record.Message}");
                }  
    }
}


void Cheep(string message)
{
    using (var writer = new StreamWriter("chirp_cli_db.csv"))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        Cheep cheep = new Cheep(Environment.UserName, message, 
                      FromDateTimeToUnix(DateTime.Now.ToString(timeFormat, CultureInfo.InvariantCulture)));

        csv.WriteRecord(cheep);
    }
}

string FromUnixTimeToDateTime(long timestamp)
{
    DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(timestamp);
    string correctFormatTimestamp = dto.ToLocalTime().ToString(timeFormat, CultureInfo.InvariantCulture);
    return correctFormatTimestamp;
}

long FromDateTimeToUnix(string dateTimeStamp)
{
    DateTime parsedTime = DateTime.Parse(dateTimeStamp, CultureInfo.InvariantCulture);
    return new DateTimeOffset(parsedTime).ToUnixTimeSeconds();
}

public record Cheep(string Author, string Message, long Timestamp);