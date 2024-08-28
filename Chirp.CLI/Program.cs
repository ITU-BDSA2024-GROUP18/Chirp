using System.Globalization;

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

    string FromUnixTimeToDateTime(string timestamp)
    {
        int timestampConverted = int.Parse(timestamp);
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(timestampConverted);
        string correctFormatTimestamp = dto.ToLocalTime().ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture);
        return correctFormatTimestamp;
    }
}