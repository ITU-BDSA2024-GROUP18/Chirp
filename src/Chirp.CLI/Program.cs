using System.Globalization;
using Chirp.CLI;
using DocoptNet;
using SimpleDb;
using System.Net.Http.Headers;
using System.Net.Http.Json;

// Create an HTTP client object
var baseURL = "http://localhost:5117";
using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
client.BaseAddress = new Uri(baseURL);

string timeFormat = "MM/dd/yy HH:mm:ss";

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

string endpoint = "cheeps";

if (arguments["read"].IsTrue)
{
    int? limit = null;
    //Check if --limit is different from null or empty string, to ensure that the absence of a limit argument,
    //does not get treated as an empty string.
    if (arguments["--limit"] != null && arguments["--limit"].ToString() != "")
    {
        limit = int.Parse(arguments["--limit"].ToString());
        endpoint += $"?limit={limit}";

    }

    // Send an asynchronous HTTP GET request and automatically construct a Cheep object from the
    // JSON object in the body of the response
    var cheeps = await client.GetFromJsonAsync<List<Cheep>>(endpoint);

    if (cheeps != null)
    {
        UserInterface.PrintCheeps(cheeps);
    }
    else
    {
        Console.WriteLine("No cheeps to read");
    }

}
else if (arguments["cheep"].IsTrue)
{
    string message = arguments["<message>"].ToString();
    if (string.IsNullOrEmpty(message))
    {
        Console.WriteLine("You are missing a message to cheep :(");
    }


    //Sends asynchrounous POST request to uri, and creates a new cheep, which is stored in the body as JSON.
    await client.PostAsJsonAsync<Cheep>("cheep", new Cheep(Environment.UserName, args[1], FromDateTimeToUnix(DateTime.Now.ToString(timeFormat, CultureInfo.InvariantCulture))));
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
