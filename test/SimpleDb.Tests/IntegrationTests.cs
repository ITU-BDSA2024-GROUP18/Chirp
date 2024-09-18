using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace SimpleDb.Tests
{
    public class CSVDatabaseIntegrationTest
    {
        private readonly string testCsvPath = "test_chirp_cli_db.csv";

        public CSVDatabaseIntegrationTest()
        {
            // Clean file before each test
            if (File.Exists(testCsvPath))
            {
                File.Delete(testCsvPath);
            }
        }

        // For testing purposes :)
        public class Cheep
        {
            public required string Author { get; set; }
            public required string Message { get; set; }
        }

        [Fact]
        public void StoreAndRetrieveEntry_Success()
        {
            // Arrange
            string db = "test_chirp_cli_db.csv";

            var database = CSVDatabase<Cheep>.Instance;
            database.Set_path(db);
            var cheep = new Cheep
            {
                Author = "Omar",
                Message = "Test message"
            };

            // Act
            database.Store(cheep);

            // Assert that the entry can be retrieved
            var cheeps = database.Read().ToList();
            Assert.Single(cheeps);
            Assert.Equal(cheep.Author, cheeps[0].Author);
            Assert.Equal(cheep.Message, cheeps[0].Message);
        }

        [Fact]
        public void StoreMultipleEntriesAndRetrieve_Success()
        {
            // Arrange
            string db = "test_chirp_cli_db.csv";
            var database = CSVDatabase<Cheep>.Instance;
            database.Set_path(db);
            var cheep1 = new Cheep
            {
                Author = "Omar",
                Message = "First message"
            };
            var cheep2 = new Cheep
            {
                Author = "Omar2",
                Message = "Second message"
            };

            // Act
            database.Store(cheep1);
            database.Store(cheep2);

            // Assert that both entries can be retrieved
            var cheeps = database.Read().ToList();
            Assert.Equal(2, cheeps.Count);
            Assert.Contains(cheeps, c => c.Author == cheep1.Author && c.Message == cheep1.Message);
            Assert.Contains(cheeps, c => c.Author == cheep2.Author && c.Message == cheep2.Message);
        }
    }
}