namespace SimpleDb;

using System.Globalization;
using CsvHelper;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    readonly string csvPath = "../Chirp.CLI/chirp_cli_db.csv";

    private CSVDatabase() { }

    public static CSVDatabase<T> Instance { get { return Nested.instance; } }

    private class Nested
    {
        static Nested() { }
        internal static readonly CSVDatabase<T> instance = new CSVDatabase<T>();
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        using (var reader = new StreamReader(csvPath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {

            //if this was not converted to a list, the IEnumerable would sort of "disappear
            //because the function only uses CsvReader until it returns. Needs more explanation
            return csv.GetRecords<T>().ToList();

        }
    }

    public void Store(T record)
    {
        using (var stream = File.Open(csvPath, FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {

            csv.NextRecord(); //acts as an "enter" button, so the new record gets written on its own line
            csv.WriteRecord(record);
        }
    }
}

