using System.Globalization;

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
    string[] lines = File.ReadAllLines("chirp_cli_db.csv");

    for (int i = 1; i < lines.Length; i++)
    {
        string[] cheepData = lines[i].Split('"');

        string author = cheepData[0].TrimEnd(',');
        string message = cheepData[1];
        string timestamp = cheepData[2].TrimStart(',');

        string dateTime = FromUnixTimeToDateTime(timestamp);

        string cheep = $"{author} @ {dateTime} : {message}";

        Console.WriteLine(cheep);
    }
}

void Cheep(string message)
{
    string author = Environment.UserName;
    long timestamp = FromDateTimeToUnix(DateTime.Now.ToString(timeFormat, CultureInfo.InvariantCulture));

    string chirp = $"{author},\"{message}\",{timestamp}";

    using StreamWriter sw = File.AppendText("chirp_cli_db.csv");
    sw.WriteLine(chirp);
}

string FromUnixTimeToDateTime(string timestamp)
{
    int timestampConverted = int.Parse(timestamp);
    DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(timestampConverted);
    string correctFormatTimestamp = dto.ToLocalTime().ToString(timeFormat, CultureInfo.InvariantCulture);
    return correctFormatTimestamp;
}

long FromDateTimeToUnix(string dateTimeStamp)
{
    DateTime parsedTime = DateTime.Parse(dateTimeStamp, CultureInfo.InvariantCulture);
    return new DateTimeOffset(parsedTime).ToUnixTimeSeconds();
}