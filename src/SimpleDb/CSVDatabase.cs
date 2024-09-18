namespace SimpleDb;

using System.Globalization;
using CsvHelper;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private string csvPath = "../Chirp.CLI/chirp_cli_db.csv";

    private static CSVDatabase<T>? instance = null;

    // public CSVDatabase(string csvPath)
    // {
    //     this.csvPath = csvPath;
    // }

    private CSVDatabase()
    {
    }

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


    public void Set_path(string path)

    {
        csvPath = path;


    }

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

