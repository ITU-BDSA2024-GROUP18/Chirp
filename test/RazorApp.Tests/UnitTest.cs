using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Repositories;
using Chirp.Core.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Humanizer;
using Chirp.Core.DTOs;
using System.Globalization;
using NuGet.Packaging.Signing;
using System.Net.Http.Headers;

namespace RazorApp.Tests

{

    public class UnitTest1 : IAsyncDisposable
    {

        private ChirpDBContext _context = null!;
        private ICheepRepository _repo = null!;
        private IAuthorRepository _authorRepo = null!;

        private SqliteConnection _connection = null!;

        //Method is private to remove warnings concerning visibility.
        private void InitMockDB()
        {
            var a1 = new Author() { Id = "13", UserName = "Tester Testerington", Email = "tt1@itu.dk", Cheeps = new List<Cheep>() };
            var a2 = new Author() { Id = "14", UserName = "Testine Testsson", Email = "tt2@itu.dk", Cheeps = new List<Cheep>() };
            var a3 = new Author() { Id = "15", UserName = "Testy Testitez", Email = "tt3@itu.dk", Cheeps = new List<Cheep>() };
            var ta1 = new Author() { Id = "16", UserName = "My Name Test", Email = "test@itu.dk", Cheeps = new List<Cheep>() };


            var authors = new List<Author>() { a1, a2, a3, ta1 };

            var c1 = new Cheep() { CheepId = 658, AuthorId = a1.Id, Author = a1, Text = "My name is Tester Testerington", TimeStamp = DateTime.Now };
            var c2 = new Cheep() { CheepId = 659, AuthorId = a2.Id, Author = a2, Text = "My name is Testine Testsson", TimeStamp = DateTime.Now };
            var c3 = new Cheep() { CheepId = 660, AuthorId = a3.Id, Author = a3, Text = "My name is Testy Testitez", TimeStamp = DateTime.Now };

            var tc1 = new Cheep() { CheepId = 661, AuthorId = ta1.Id, Author = ta1, Text = "This is my first cheep", TimeStamp = DateTime.Now };
            var tc2 = new Cheep() { CheepId = 662, AuthorId = ta1.Id, Author = ta1, Text = "This is my second cheep", TimeStamp = DateTime.Now };
            var tc3 = new Cheep() { CheepId = 663, AuthorId = ta1.Id, Author = ta1, Text = "This is my third cheep", TimeStamp = DateTime.Now };

            var cheeps = new List<Cheep>() { c1, c2, c3, tc1, tc2, tc3 };
            a1.Cheeps = new List<Cheep>() { c1 };
            a2.Cheeps = new List<Cheep>() { c2 };
            a3.Cheeps = new List<Cheep>() { c3 };
            ta1.Cheeps = new List<Cheep>() { tc1, tc2, tc3 };

            _context.Authors.AddRange(authors);
            _context.Cheeps.AddRange(cheeps);
            _context.SaveChanges();
        }

        // Tear down method. Xunit will call this method everytime a test is complete due to the implementation of
        // the interface 'IAsyncDisposable'. This ensures that everytime we complete a test, we tear down connection and context
        // such that we can call startmock(); at the start of every method.
        public async ValueTask DisposeAsync()
        {
            if (_context != null)
            {
                await _context.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
        }

        //Method is private to remove warnings concerning visibility.
        private async Task StartMockDB()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            await _connection.OpenAsync();
            var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(_connection);
            //builder.EnableSensitiveDataLogging();

            _context = new ChirpDBContext(builder.Options);
            await _context.Database.EnsureCreatedAsync(); // Applies the schema to the database

            _repo = new CheepRepository(_context);
            _authorRepo = new AuthorRepository(_context);
        }

        //Latest Id: 12
        //Latest CheepId: 657

        [Fact]
        public async Task AddedCheep_IsSavedToDB()
        {

            //Arrange
            var ta1 = new Author() { Id = "13", UserName = "My Name Test", Email = "test@itu.dk", Cheeps = new List<Cheep>() };

            var tc1 = new Cheep() { CheepId = 658, AuthorId = ta1.Id, Author = ta1, Text = "This is my first cheep", TimeStamp = DateTime.Now };

            //Act
            await StartMockDB();
            await _repo.AddCheep(tc1);

            //Assert
            var actualCheep = await _context.Cheeps.Where(cheep => cheep.AuthorId == tc1.AuthorId).FirstOrDefaultAsync();
            Assert.Equal("13", actualCheep!.Author.Id);
            Assert.Equal("My Name Test", actualCheep.Author.UserName);
            Assert.Equal("test@itu.dk", actualCheep.Author.Email);
            Assert.Equal("This is my first cheep", actualCheep.Text);
        }

        [Fact]
        public async Task Cheep_IsDeletedFromDB()
        {

            //Arrange
            var ta1 = new Author() { Id = "13", UserName = "My Name Test", Email = "test@itu.dk", Cheeps = new List<Cheep>() };

            var tc1 = new Cheep() { CheepId = 658, AuthorId = ta1.Id, Author = ta1, Text = "This is my first cheep", TimeStamp = DateTime.Now };

            //Act
            await StartMockDB();
            await _repo.AddCheep(tc1);


            var CheepDTO = new CheepDTO
            {

                AuthorName = tc1.AuthorId,
                Message = tc1.Text,
                Timestamp = tc1.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)

            };

            await _repo.DeleteCheeps(CheepDTO.AuthorName, CheepDTO.Timestamp, CheepDTO.Message);

            //actualCheep = await _context.Cheeps.Where(cheep => cheep.CheepId == 658).FirstOrDefaultAsync();

            //Assert
            //if no record is found FirstOrDefaultAsync will return default value, which is NULL for our cheeps
            var actualCheep = await _context.Cheeps.Where(cheep => cheep.CheepId == 658).FirstOrDefaultAsync();

            Assert.Null(actualCheep);
        }

        [Fact]
        public async Task AddedAuthor_IsSavedToDB()
        {
            //Arrange
            var ta1 = new Author() { Id = "13", UserName = "My Name Test", Email = "test@itu.dk", Cheeps = new List<Cheep>() };

            //Act
            await StartMockDB();
            await _authorRepo.AddAuthor(ta1);

            //Assert
            var actualAuthor = await _context.Authors.Where(auth => auth.Id == ta1.Id).FirstOrDefaultAsync();
            Assert.Equal("My Name Test", actualAuthor!.UserName);
        }

        [Theory]
        [InlineData("Jacqualine Gilcoine", "Jacqualine Gilcoine", "10")]
        [InlineData("Octavio Wagganer", "Octavio Wagganer", "8")]
        public async Task GetAuthorByName_ReturnsCorrectAuthorName(string Name, string expectedName, string id)
        {

            //Act
            await StartMockDB();

            //Assert
            var actualAuthor = await _authorRepo.GetAuthorByName(Name);
            Assert.Equal(expectedName, actualAuthor.Username);
            Assert.Equal(id, actualAuthor.Id);
        }

        // [Fact]
        // public async Task CheckAuthorExists_ThrowsExceptionWhenNotFound()
        // {
        //     //Arrange
        //     var Id = 13;

        //     //Act
        //     await StartMockDB();

        //     //Assert
        //     var actualException = await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _repo.CheckAuthorExists(Id));
        //     Assert.Equal($"Author with ID {Id} does not exist.", actualException.Message);
        // }

        [Fact]
        public async Task CheckAuthorExists_ReturnsAuthor()
        {

            //Act
            await StartMockDB();

            //Assert
            var actualAuthor = await _authorRepo.CheckAuthorExists("12");
            Assert.Equal("Adrian", actualAuthor?.UserName);
        }

        [Fact]
        public async Task GetLatestIdCheep_ReturnsLastAddedCheepId()
        {

            //Act
            await StartMockDB();
            //InitMock adds multiple authors and cheeps - highest cheepId after initmock() is 663
            InitMockDB();

            //Assert
            var actualCheepId = await _repo.GetLatestIdCheep();
            Assert.Equal(663, actualCheepId);
        }

        [Fact]
        public async Task GetLatestIdAuthor_ReturnsLastAddedId()
        {

            //Act
            await StartMockDB();
            //InitMock adds multiple authors and cheeps - highest Id after initmock() is 16
            InitMockDB();

            //Assert
            var actualId = await _authorRepo.GetLatestIdAuthor();
            Assert.Equal("16", actualId);
        }

        [Fact]
        public async Task ReadFromAuthor_ReturnsAllCheepsByAuthor()
        {

            //Act
            await StartMockDB();
            InitMockDB();
            var actualAuthorCheeps = await _repo.ReadFromAuthor(1, "My Name Test");

            //Assert
            Assert.All(actualAuthorCheeps, testcheeps => Assert.Equal("My Name Test", testcheeps.AuthorName));
        }

        [Fact]
        public async Task ReadFromAuthor_ReturnsEmptyList_NonExistantAuthor()
        {

            //Act
            await StartMockDB();
            var actualAuthorCheeps = await _repo.ReadFromAuthor(1, "Non Existant Author");

            //Assert
            Assert.Empty(actualAuthorCheeps);
        }

        //The test below will fail if we have a future db with many cheeps, so we should consider correctly mocking a clean empty version of the db.
        [Fact]
        public async Task ReadPublicTimeline_ReturnsEmptyList_NoCheepsOnPage()
        {

            //Act
            await StartMockDB();
            var actualPageCheeps = await _repo.ReadPublicTimeline(1000);

            //Assert
            Assert.Empty(actualPageCheeps);
        }

        [Theory]

        [InlineData(5, 32)]
        [InlineData(21, 17)]
        public async Task EnsureCorrectCheepsPerpage(int pagenum, int expectedNumOfCheeps)
        {
            //pagenumber should contain 32 cheeps, and pagenum 21 should contain 17 cheeps currently

            //act
            await StartMockDB();
            var actualPageCheeps = await _repo.ReadPublicTimeline(pagenum);

            Assert.Equal(expectedNumOfCheeps, actualPageCheeps.Count);
        }


        [Theory]
        [InlineData("Donald", "testing@itu.dk", "13")]

        public async Task CreateAuthor(string Name, string Email, string expectedAuthorId)
        {

            //Arrange 
            await StartMockDB();

            AuthorService authorservice = new AuthorService(_authorRepo);
            //Act
            //CreateAuthor should get the latestID present in the database, and increment it by 1
            var newAuthor = await authorservice.CreateAuthor(Name, Email);

            //Assert
            Assert.Equal(expectedAuthorId, newAuthor.Id);
            Assert.Equal(Name, newAuthor.Username);
        }

        [Theory]
        [InlineData("12", "Creating Cheep to Adrian", 658)]

        public async Task CreateCheep_WhereAuthorExists(string authorid, string message, int expectedCheepId)
        {
            //Arrange 
            await StartMockDB();

            CheepService cheepservice = new CheepService(_repo, _authorRepo);

            //Act
            var newCheepForAdrian = await cheepservice.CreateCheep(authorid, message);

            //Assert 
            Assert.Equal(authorid, newCheepForAdrian.AuthorId);
            Assert.Equal(message, newCheepForAdrian.Text);
            Assert.Equal(expectedCheepId, newCheepForAdrian.CheepId);
        }


        // [Fact]
        // public void FromUnixTimeToDateTime_ConvertsCorrectly()
        // {
        //     // Arrange
        //     double unixTime = 1728383396; //Unixtimestamp for: 12:29:50 08-10-2024

        //     //Act 
        //     string actualDateTime = _repo.UnixTimeStampToDateTimeString(unixTime);

        //     // Assert
        //     Assert.Equal("08/10/24 10:29:56", actualDateTime);
        // }
    }

}

