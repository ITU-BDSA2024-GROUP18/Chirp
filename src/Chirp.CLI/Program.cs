using System.Globalization;
using Chirp.CLI;
using DocoptNet;
using SimpleDb;

string timeFormat = "MM/dd/yy HH:mm:ss";

IDatabaseRepository<Cheep> db = CSVDatabase<Cheep>.Instance;

const string usage = @"Chirp.CLI Version.

    Usage:
        chirp read
        chirp cheep <message>
        chirp help
        chirp --version

    Options:
        help     Displays usage options
        --version  Show version.";

var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;


if (arguments["read"].IsTrue)
{
    UserInterface.PrintCheeps(db.Read());
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