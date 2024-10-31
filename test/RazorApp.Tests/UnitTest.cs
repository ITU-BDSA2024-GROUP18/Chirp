using Chirp.Razor;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


namespace RazorApp.Tests
{

    public class UnitTest1
    {

        private ChirpDBContext _context = null!;
        private ICheepRepository _repo = null!;

        public void InitMockDB()
        {
            var a1 = new Author() { AuthorId = 20, Name = "Tester Testerington", Email = "tt1@itu.dk", Cheeps = new List<Cheep>() };
            var a2 = new Author() { AuthorId = 21, Name = "Testine Testsson", Email = "tt2@itu.dk", Cheeps = new List<Cheep>() };
            var a3 = new Author() { AuthorId = 22, Name = "Testy Testitez", Email = "tt3@itu.dk", Cheeps = new List<Cheep>() };

            var authors = new List<Author>() { a1, a2, a3 };

            var c1 = new Cheep() { CheepId = 900, AuthorId = a1.AuthorId, Author = a1, Text = "My name is Tester Testerington", TimeStamp = DateTime.Now };
            var c2 = new Cheep() { CheepId = 901, AuthorId = a2.AuthorId, Author = a2, Text = "My name is Testine Testsson", TimeStamp = DateTime.Now };
            var c3 = new Cheep() { CheepId = 902, AuthorId = a3.AuthorId, Author = a3, Text = "My name is Testy Testitez", TimeStamp = DateTime.Now };

            var cheeps = new List<Cheep>() { c1, c2, c3 };
            a1.Cheeps = new List<Cheep>() { c1 };
            a2.Cheeps = new List<Cheep>() { c2 };
            a3.Cheeps = new List<Cheep>() { c3 };

            _context.Authors.AddRange(authors);
            _context.Cheeps.AddRange(cheeps);
            _context.SaveChanges();
        }

        public async Task StartMockDB()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            await connection.OpenAsync();
            var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);
            //builder.EnableSensitiveDataLogging();

            _context = new ChirpDBContext(builder.Options);
            await _context.Database.EnsureCreatedAsync(); // Applies the schema to the database

            _repo = new CheepRepository(_context);
        }

        [Fact]
        public async Task AddedCheep_IsSavedToDB()
        {

            //Arrange
            var ta1 = new Author() { AuthorId = 100, Name = "My Name Test", Email = "test@itu.dk", Cheeps = new List<Cheep>() };

            var tc1 = new Cheep() { CheepId = 999, AuthorId = ta1.AuthorId, Author = ta1, Text = "This is my first cheep", TimeStamp = DateTime.Now };

            //Act
            await StartMockDB();
            await _repo.AddCheep(tc1);

            //Assert
            var actualCheep = await _context.Cheeps.Where(cheep => cheep.AuthorId == tc1.AuthorId).FirstOrDefaultAsync();
            Assert.Equal(100, actualCheep.Author.AuthorId);
            Assert.Equal("My Name Test", actualCheep.Author.Name);
            Assert.Equal("test@itu.dk", actualCheep.Author.Email);
            Assert.Equal("This is my first cheep", actualCheep.Text);
        }

        [Fact]
        public async Task AddedAuthor_IsSavedToDB()
        {
            //Arrange
            var ta1 = new Author() { AuthorId = 100, Name = "My Name Test", Email = "test@itu.dk", Cheeps = new List<Cheep>() };

            //Act
            await StartMockDB();
            await _repo.AddAuthor(ta1);

            //Assert
            var actualAuthor = await _context.Authors.Where(auth => auth.AuthorId == ta1.AuthorId).FirstOrDefaultAsync();
            Assert.Equal("My Name Test", actualAuthor.Name);
        }

        [Fact]
        public async Task GetAuthorByEmail_ReturnsCorrectAuthor()
        {
            //Arrange
            var ta1 = new Author() { AuthorId = 100, Name = "My Name Test", Email = "test@itu.dk", Cheeps = new List<Cheep>() };

            //Act
            await StartMockDB();
            await _repo.AddAuthor(ta1);

            //Assert
            var actualAuthor = await _repo.GetAuthorByEmail("test@itu.dk");
            Assert.Equal("test@itu.dk", actualAuthor.Email);
        }

        [Fact]
        public async Task GetAuthorByName_ReturnsCorrectAuthorName()
        {
            //Arrange
            var ta1 = new Author() { AuthorId = 100, Name = "My Name Test", Email = "test@itu.dk", Cheeps = new List<Cheep>() };

            //Act
            await StartMockDB();
            await _repo.AddAuthor(ta1);

            //Assert
            var actualAuthor = await _repo.GetAuthorByName("My Name Test");
            Assert.Equal("My Name Test", actualAuthor.Name);
        }

        [Fact]
        public async Task CheckAuthorExists_ThrowsExceptionWhenNotFound()
        {
            //Arrange
            var authorId = 99;

            //Act
            await StartMockDB();

            //Assert
            var actualException = await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _repo.CheckAuthorExists(authorId));
            Assert.Equal($"Author with ID {authorId} does not exist.", actualException.Message);
        }

        [Fact]
        public async Task CheckAuthorExists_ReturnsAuthor()
        {
            //Arrange

            //Act
            await StartMockDB();
            InitMockDB();

            //Assert
            var actualAuthor = await _repo.CheckAuthorExists(20);
            Assert.Equal("Tester Testerington", actualAuthor.Name);
        }

        [Fact]
        public async Task GetLatestIdCheep_ReturnsLastAddedCheepId()
        {
            //Arrange
            var ta1 = new Author() { AuthorId = 100, Name = "My Name Test", Email = "test@itu.dk", Cheeps = new List<Cheep>() };

            var tc1 = new Cheep() { CheepId = 999, AuthorId = ta1.AuthorId, Author = ta1, Text = "This is my first cheep", TimeStamp = DateTime.Now };

            //Act
            await StartMockDB();
            await _repo.AddCheep(tc1);

            //Assert
            var actualCheepId = await _repo.GetLatestIdCheep();
            Assert.Equal(999, actualCheepId);
        }


        [Fact]
        public void FromUnixTimeToDateTime_ConvertsCorrectly()
        {
            // Arrange
            double unixTime = 1728383396; //Unixtimestamp for: 12:29:50 08-10-2024

            //Act 
            string actualDateTime = DbFacade.UnixTimeStampToDateTimeString(unixTime);

            // Assert
            Assert.Equal("08/10/24 10.29.56", actualDateTime);
        }
    }

}

