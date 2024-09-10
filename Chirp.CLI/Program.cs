using System.Globalization;
using CsvHelper;
using SimpleDb;

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
    Store(args[1]);
}