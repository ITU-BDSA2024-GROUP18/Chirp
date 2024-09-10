namespace SimpleDb;

using System.Globalization;
using CsvHelper;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    readonly string csvPath = "../Chirp.CLI/chirp_cli_db.csv";

    public IEnumerable<T> Read(int? limit = null)
    {
        using (var reader = new StreamReader(csvPath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<T>();
            //For printing to terminal:
            /*foreach (var record in csv.GetRecords<Cheep>())
                    {
                        Console.WriteLine($"{record.Author} @ {FromUnixTimeToDateTime(record.Timestamp)} : {record.Message}");
                    }  */
        }
    }

    public void Store(T record)
    {
        using (var stream = File.Open(csvPath, FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            /*Cheep cheep = new Cheep(Environment.UserName, message, 
                        FromDateTimeToUnix(DateTime.Now.ToString(timeFormat, CultureInfo.InvariantCulture))); */

            csv.NextRecord();
            csv.WriteRecord(record);
        }
    }
}

