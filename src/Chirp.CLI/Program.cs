using System.Globalization;
using Chirp.CLI;
using DocoptNet;
using SimpleDb;

string timeFormat = "MM/dd/yy HH:mm:ss";

IDatabaseRepository<Cheep> db = new CSVDatabase<Cheep>();

const string usage = @"Chirp.CLI Version.

    Usage:
        chirp read [--limit=<n>]
        chirp cheep <message>
        chirp help
        chirp --version

    Options:
        --limit=<n>  Limit the number of cheeps to read (default: no limit).
        help     Displays usage options
        --version  Show version.";

var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;


if (arguments["read"].IsTrue)
{
    int? limit = null;
    if (arguments["--limit"] != null)
    {
        limit = int.Parse(arguments["--limit"].ToString());
    }
    UserInterface.PrintCheeps(db.Read(limit));
}
else if (arguments["cheep"].IsTrue)
{
    string message = arguments["<message>"].ToString();
    if (string.IsNullOrEmpty(message))
    {
        Console.WriteLine("You are missing a message to cheep :(");
    }

    db.Store(new Cheep(Environment.UserName, args[1],
             FromDateTimeToUnix(DateTime.Now.ToString(timeFormat, CultureInfo.InvariantCulture))));
}
else if (arguments["help"].IsTrue)
{
    Console.WriteLine(usage);
}

long FromDateTimeToUnix(string dateTimeStamp)
{
    DateTime parsedTime = DateTime.Parse(dateTimeStamp, CultureInfo.InvariantCulture);
    return new DateTimeOffset(parsedTime).ToUnixTimeSeconds();
}

public record Cheep(string Author, string Message, long Timestamp);