using System.ComponentModel.Design;
using System.Globalization;
using SimpleDb;

string timeFormat = "MM/dd/yy HH:mm:ss";

IDatabaseRepository<Cheep> db = new CSVDatabase<Cheep>();

if (args.Length < 1) 
{  
    Console.WriteLine("How to use the program: \n" + 
                    " dotnet run -- <read> (Displays all cheeps in DB) \n" + 
                    " dotnet run -- <cheep> [message] (Cheep a message to the DB)");
}

if (args[0] == "read")
{
    db.Read();
} 
else if (args[0]  == "cheep")
{
    if (args.Length < 2)
    {
        Console.WriteLine("You are missing a message to cheep :(");
    }

    db.Store(new Cheep(Environment.UserName, args[1], 
             FromDateTimeToUnix(DateTime.Now.ToString(timeFormat, CultureInfo.InvariantCulture))));
}

    long FromDateTimeToUnix(string dateTimeStamp)
    {
        DateTime parsedTime = DateTime.Parse(dateTimeStamp, CultureInfo.InvariantCulture);
        return new DateTimeOffset(parsedTime).ToUnixTimeSeconds();
    }

    //For printing purposes.
    /*string FromUnixTimeToDateTime(long timestamp)
        {
            DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            string correctFormatTimestamp = dto.ToLocalTime().ToString(timeFormat, CultureInfo.InvariantCulture);
            return correctFormatTimestamp;
        }
    */

record Cheep(string Author, string Message, long Timestamp);