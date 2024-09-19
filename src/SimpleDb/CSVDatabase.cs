namespace SimpleDb;

using System.Globalization;
using CsvHelper;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    //Singleton pattern starts
    private string csvPath = "../Chirp.CLI/chirp_cli_db.csv";

    private static CSVDatabase<T>? instance = null;

    //A single constructor, which is private and parameterless. 
    //This prevents other classes from instantiating it (which would be a violation of the pattern)
    private CSVDatabase()
    {
    }
    //creates an instance of database - It is not thread safe, as many threads could evaluate line_23 to true, concurrently, 
    //but its ok for now!
    public static CSVDatabase<T> Instance
    {
        get
        {
            if (instance == null)
            {

                instance = new CSVDatabase<T>();
            }
            return instance;
        }
    }

    //Method below can override the field csvPath which has been set when an instance is created
    public void Set_path(string path)

    {
        csvPath = path;


    }
    //Singleton pattern ends

    public IEnumerable<T> Read(int? limit = null)
    {
        using (var reader = new StreamReader(csvPath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            //if this was not converted to a list, the IEnumerable would sort of "disappear
            //because the function only uses CsvReader until it returns. Needs more explanation
            var records = csv.GetRecords<T>().ToList();

            if (limit.HasValue)
            {
                return records.Take(limit.Value).ToList();
            }
            else
            {
                return records;
            }
        }
    }

    public void Store(T record)
    {
        var fileExists = File.Exists(csvPath);

        using (var stream = File.Open(csvPath, FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            if (!fileExists)
            {
                // Write the header only if the file does not exist
                csv.WriteHeader<T>();
                csv.NextRecord();
            }

            csv.NextRecord(); //acts as an "enter" button, so the new record gets written on its own line
            csv.WriteRecord(record);
        }
    }
}

