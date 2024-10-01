using System.Data;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;


namespace Chirp.Razor; 


public class DbFacade
    {

    private readonly string _connection_string;

    //Embedded SQLite scripts, to be used if path from connection_string does not exist
    private readonly IFileProvider embeddedFile; 


    //Initialize DbFacade
    public DbFacade(string connection_string)
    {

        //Set the path for connecting to the database
        _connection_string = connection_string;

        //Create a connection to the SQLite database
        var connection = new SqliteConnection(connection_string);

        //Should only use embedded file when connection_string to database 
        //embeddedFile = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
    }

    }


    
