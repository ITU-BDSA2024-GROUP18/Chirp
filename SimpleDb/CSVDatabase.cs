namespace SimpleDb;

using System.Globalization;
using CsvHelper;

sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    string timeFormat = "MM/dd/yy HH:mm:ss";
    string csvPath = "../Chirp.CLI/chirp_cli_db.csv";

    public IEnumerable<T> Read(int? limit = null)
    {
        using (var reader = new StreamReader(csvPath))
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


    public void Store(T message)
    {
        using (var writer = new StreamWriter(csvPath))
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
}

