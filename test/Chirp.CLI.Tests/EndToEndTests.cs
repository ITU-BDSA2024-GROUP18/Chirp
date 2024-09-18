using Xunit;
using System.IO;
using System;
using SimpleDb;
using Chirp.CLI;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public class ChirpCliTests
{
    private const string TestCsvPath = "test_chirp_cli_db.csv";
    private const string TimeFormat = "MM/dd/yy HH:mm:ss";

    public ChirpCliTests()
    {
        CreateTestCsv();
    }

    [Fact]
    public void TestReadWithLimit()
    {
        // Arrange
        var csvDatabase = CSVDatabase<Cheep>.Instance;
        csvDatabase.Set_path(TestCsvPath);
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter); // Redirect console output to StringWriter

        // Act
        UserInterface.PrintCheeps(csvDatabase.Read(1));

        // Assert
        var output = stringWriter.ToString().Trim();
        Assert.Contains("Omar @ 01/01/24 00:00:00: Message 1", output);
        Assert.DoesNotContain("Asger", output);

        // Cleanup
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
        stringWriter.Dispose();
    }

    [Fact]
    public void TestCheepWithMessage()
    {
        // Arrange
        var csvDatabase = CSVDatabase<Cheep>.Instance;
        csvDatabase.Set_path(TestCsvPath);
        var testCheep = new Cheep("Author", "New message!", 1726660160);

        // Act
        csvDatabase.Store(testCheep);

        var allCheeps = csvDatabase.Read().ToList(); // Read all cheeps to find the new one
        var output = allCheeps.Find(cheep => cheep.Author == "Author" && cheep.Message == "New message!");

        // Assert
        Assert.NotNull(output);
        Assert.Equal("New message!", output.Message);
    }

    private void CreateTestCsv()
    {
        var cheeps = new List<Cheep>
        {
            new Cheep("Omar", "Message 1", ConvertToUnixTimestamp("01/01/24 00:00:00")),
            new Cheep("Asger", "Message 2", ConvertToUnixTimestamp("01/02/24 00:00:00")),
            new Cheep("Lukas", "Message 3", ConvertToUnixTimestamp("01/03/24 00:00:00"))
        };

        using (var writer = new StreamWriter(File.Open(TestCsvPath, FileMode.Create)))
        using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(cheeps);
        }
    }

    private long ConvertToUnixTimestamp(string dateTimeStr)
    {
        var dateTime = DateTime.ParseExact(dateTimeStr, TimeFormat, CultureInfo.InvariantCulture);
        return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
    }

    [Fact]
    public void Dispose()
    {
        File.Delete(TestCsvPath);
    }
}