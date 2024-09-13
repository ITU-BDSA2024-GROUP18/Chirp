namespace Chirp.CLI;

using System.Globalization;



public static class UserInterface
{
    const string timeFormat = "MM/dd/yy HH:mm:ss";
    public static void PrintCheeps(IEnumerable<Cheep> cheepList)
    {
        foreach (Cheep cheep in cheepList)
        {
            Console.WriteLine($"{cheep.Author} @ {FromUnixTimeToDateTime(cheep.Timestamp)}: {cheep.Message}");
        }
    }

    //For printing purposes.
    private static string FromUnixTimeToDateTime(long timestamp)
    {
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        string correctFormatTimestamp = dto.ToLocalTime().ToString(timeFormat, CultureInfo.InvariantCulture);
        return correctFormatTimestamp;
    }
}