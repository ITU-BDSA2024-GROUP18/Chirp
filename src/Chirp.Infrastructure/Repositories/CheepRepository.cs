using System.Globalization;
using Chirp.Core.DTOs;
using Chirp.Core.Entities;
using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;

    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
        dbContext.Database.EnsureCreated();
        DbInitializer.SeedDatabase(_dbContext);
    }

    // Queries
    public async Task<List<CheepDTO>> ReadPublicTimeline(int pagenum)
    {
        var query =
            from cheeps in _dbContext.Cheeps
            orderby cheeps.TimeStamp descending
            select new
            {
                cheeps.Author,
                cheeps.Text,
                cheeps.TimeStamp
            };

        var result = await query.Skip((pagenum - 1) * 32).Take(32).ToListAsync();

        return result.Select(cheep => new CheepDTO
        {
            AuthorName = cheep.Author?.UserName ?? "Unknown Author", // Handle null here
            Message = cheep.Text,
            Timestamp = cheep.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
        }).ToList();
    }


    public async Task<List<CheepDTO>> ReadFromFollows(int pagenum, string author)
    {

        var query =

            from cheeps in _dbContext.Cheeps
                //Include cheeps of the logged in author
            where cheeps.Author.UserName == author || (

            //Get the IDs of who the current author follows
            //and use that to include their cheeps
                from authors in _dbContext.Authors
                where authors.UserName == author
                from follow in authors.Follows
                select follow).Contains(cheeps.Author)
            orderby cheeps.TimeStamp descending
            select new
            {

                Author = cheeps.Author,
                Text = cheeps.Text,
                TimeStamp = cheeps.TimeStamp

            };

        var result = await query.Skip((pagenum - 1) * 32).Take(32).ToListAsync();

        return result.Select(cheep => new CheepDTO
        {
            AuthorName = cheep.Author?.UserName ?? "Unknown Author", // Handle null here
            Message = cheep.Text,
            Timestamp = cheep.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
        }).ToList();



    }

    public async Task<int> GetLatestIdCheep()
    {
        var query = _dbContext.Cheeps.OrderByDescending(c => c.CheepId).Select(c => c.CheepId);
        return await query.FirstOrDefaultAsync();
    }

    public async Task AddCheep(Cheep cheep)
    {
        await _dbContext.Cheeps.AddAsync(cheep);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCheep(string? authorid, string timestamp, string message)
    {

        //Fetch all cheeps from an author that has a match between DTO message and cheep text
        var cheeps = _dbContext.Cheeps
            .Where(c => c.AuthorId == authorid && c.Text == message)
            .ToList();


        var cheepToDelete = cheeps.SingleOrDefault(c => c.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) == timestamp);

        if (cheepToDelete != null)
        {

            _dbContext.Cheeps.Remove(cheepToDelete);
            await _dbContext.SaveChangesAsync();

        }
    }



}