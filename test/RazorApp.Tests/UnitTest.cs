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
        private AuthorRepository _authorRepository = null!;
        private AuthorService _authorService = null!;

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
            _authorRepository = new AuthorRepository(_context);
            _authorService = new AuthorService(_authorRepo);
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

            await _repo.DeleteCheep(CheepDTO.AuthorName, CheepDTO.Timestamp, CheepDTO.Message);

            //actualCheep = await _context.Cheeps.Where(cheep => cheep.CheepId == 658).FirstOrDefaultAsync();

            //Assert
            //if no record is found FirstOrDefaultAsync will return default value, which is NULL for our cheeps
            var actualCheep = await _context.Cheeps.Where(cheep => cheep.CheepId == 658).FirstOrDefaultAsync();

            Assert.Null(actualCheep);
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

        [Fact]
        public async Task GetCheeps_ReturnsCheepsForGivenPage()
        {
            // Arrange
            await StartMockDB();
            InitMockDB();
            CheepService cheepService = new CheepService(_repo, _authorRepo);

            // Act
            var result = await cheepService.GetCheeps(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(32, result.Count);
        }

        [Fact]
        public async Task GetCheeps_ReturnsEmptyList_WhenNoCheepsExistForPage()
        {
            // Arrange
            await StartMockDB();
            var cheepService = new CheepService(_repo, _authorRepo);

            // Act
            var result = await cheepService.GetCheeps(22); // Page 22 has no cheeps in mock data

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task Follows_ShouldReturnTrueIfUserIsFollowing()
        {
            // Arrange
            await StartMockDB();
            InitMockDB();

            var user1 = await _context.Authors.FirstAsync(a => a.UserName == "Tester Testerington");
            var user2 = await _context.Authors.FirstAsync(a => a.UserName == "Testine Testsson");
            user1.Follows.Add(user2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authorRepository.Follows("Tester Testerington", "Testine Testsson");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Follow_ShouldAddFollowedUser()
        {
            // Arrange
            await StartMockDB();
            InitMockDB();

            // Act
            await _authorRepository.Follow("Tester Testerington", "Testine Testsson");

            // Assert
            var user1 = await _context.Authors.Include(a => a.Follows).FirstAsync(a => a.UserName == "Tester Testerington");
            Assert.Contains(user1.Follows, a => a.UserName == "Testine Testsson");
        }

        [Fact]
        public async Task Unfollow_ShouldRemoveFollowedUser()
        {
            // Arrange
            await StartMockDB();
            InitMockDB();

            var user1 = await _context.Authors.FirstAsync(a => a.UserName == "Tester Testerington");
            var user2 = await _context.Authors.FirstAsync(a => a.UserName == "Testine Testsson");
            user1.Follows.Add(user2);
            await _context.SaveChangesAsync();

            // Act
            await _authorRepository.Unfollow("Tester Testerington", "Testine Testsson");

            // Assert
            var updatedUser1 = await _context.Authors.Include(a => a.Follows).FirstAsync(a => a.UserName == "Tester Testerington");
            Assert.DoesNotContain(updatedUser1.Follows, a => a.UserName == "Testine Testsson");
        }

        [Fact]
        public async Task GetFollowedUsers_ShouldReturnFollowedUsers()
        {
            // Arrange
            await StartMockDB();
            InitMockDB();

            var user1 = await _context.Authors.FirstAsync(a => a.UserName == "Tester Testerington");
            var user2 = await _context.Authors.FirstAsync(a => a.UserName == "Testine Testsson");
            var user3 = await _context.Authors.FirstAsync(a => a.UserName == "Testy Testitez");

            user1.Follows.Add(user2);
            user1.Follows.Add(user3);
            await _context.SaveChangesAsync();

            // Act
            var followedUsers = await _authorRepository.GetFollowedUsers(user1.Id);

            // Assert
            Assert.Contains("Testine Testsson", followedUsers);
            Assert.Contains("Testy Testitez", followedUsers);
            Assert.Equal(2, followedUsers.Count);
        }

        [Fact]
        public async Task Follow_ShouldAddFollowedUserViaService()
        {
            // Arrange
            await StartMockDB();
            InitMockDB();

            // Act
            await _authorService.Follow("Tester Testerington", "Testine Testsson");

            // Assert
            var user1 = await _context.Authors.Include(a => a.Follows).FirstAsync(a => a.UserName == "Tester Testerington");
            Assert.Contains(user1.Follows, a => a.UserName == "Testine Testsson");
        }

        [Fact]
        public async Task Unfollow_ShouldRemoveFollowedUserViaService()
        {
            // Arrange
            await StartMockDB();
            InitMockDB();

            var user1 = await _context.Authors.FirstAsync(a => a.UserName == "Tester Testerington");
            var user2 = await _context.Authors.FirstAsync(a => a.UserName == "Testine Testsson");
            user1.Follows.Add(user2);
            await _context.SaveChangesAsync();

            // Act
            await _authorService.Unfollow("Tester Testerington", "Testine Testsson");

            // Assert
            var updatedUser1 = await _context.Authors.Include(a => a.Follows).FirstAsync(a => a.UserName == "Tester Testerington");
            Assert.DoesNotContain(updatedUser1.Follows, a => a.UserName == "Testine Testsson");
        }

        [Fact]
        public async Task GetFollowedUsers_ShouldReturnFollowedUsersViaService()
        {
            // Arrange
            await StartMockDB();
            InitMockDB();

            var user1 = await _context.Authors.FirstAsync(a => a.UserName == "Tester Testerington");
            var user2 = await _context.Authors.FirstAsync(a => a.UserName == "Testine Testsson");
            var user3 = await _context.Authors.FirstAsync(a => a.UserName == "Testy Testitez");

            user1.Follows.Add(user2);
            user1.Follows.Add(user3);
            await _context.SaveChangesAsync();

            // Act
            var followedUsers = await _authorService.GetFollowedUsers(user1.Id);

            // Assert
            Assert.Contains("Testine Testsson", followedUsers);
            Assert.Contains("Testy Testitez", followedUsers);
            Assert.Equal(2, followedUsers.Count);
        }

        [Fact]
        public async Task Follows_ShouldReturnTrueIfUserIsFollowingViaService()
        {
            // Arrange
            await StartMockDB();
            InitMockDB();

            var user1 = await _context.Authors.FirstAsync(a => a.UserName == "Tester Testerington");
            var user2 = await _context.Authors.FirstAsync(a => a.UserName == "Testine Testsson");
            user1.Follows.Add(user2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authorService.Follows("Tester Testerington", "Testine Testsson");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Follows_ShouldReturnFalseIfUserIsNotFollowingViaService()
        {
            // Arrange
            await StartMockDB();
            InitMockDB();

            // Act
            var result = await _authorService.Follows("Tester Testerington", "Testine Testsson");

            // Assert
            Assert.False(result);
        }
    }
}