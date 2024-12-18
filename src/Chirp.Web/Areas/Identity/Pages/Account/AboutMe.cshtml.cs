using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Chirp.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Services;

namespace Chirp.Web.Areas.Identity.Pages.Account;

/// <summary>
/// PageModel for the "About Me" page, managing user profile and account operations.
/// </summary>
[Authorize]
public class AboutMeModel : PageModel
{
    private readonly ChirpDBContext _dbContext;
    private readonly IAuthorService _AuthorService;

    /// <summary>
    /// Initializes a new instance of the AboutMeModel with database and author services.
    /// </summary>
    /// <param name="dbContext">The database context to use.</param>
    /// <param name="authorService">The service for author-related operations.</param>
    public AboutMeModel(ChirpDBContext dbContext, IAuthorService authorService)
    {
        _dbContext = dbContext;
        _AuthorService = authorService;
    }

    /// <summary>
    /// The username of the current user.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The email address of the current user.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// List of Cheeps posted by the current user.
    /// </summary>
    public List<Cheep> UserCheeps { get; set; } = new();

    /// <summary>
    /// List of usernames the current user is following.
    /// </summary>
    public List<string> Following { get; set; } = new();

    /// <summary>
    /// Handles the GET request to populate the "About Me" page with user data.
    /// </summary>
    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return;

        // Fetch user data
        var user = await _dbContext.Users.FindAsync(userId);
        if (user != null)
        {
            UserName = user.UserName;
            Email = user.Email;
        }

        // Fetch cheeps posted by user
        UserCheeps = _dbContext.Cheeps.Where(c => c.AuthorId == userId).ToList();

        // Fetch the list of users being followed
        Following = await _AuthorService.GetFollowedUsers(userId);
    }

    /// <summary>
    /// Handles the POST request to remove all user data ("Forget Me" operation).
    /// </summary>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    public async Task<IActionResult> OnPostForgetMeAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return BadRequest();

        // Fetch the user and related data
        var user = await _dbContext.Authors
            .Include(a => a.Follows)
            .FirstOrDefaultAsync(a => a.Id == userId);
        if (user == null) return NotFound();

        // Remove follow relationships
        if (user.Follows != null && user.Follows.Any())
        {
            _dbContext.Entry(user).Collection(u => u.Follows).Load();
            user.Follows.Clear();
        }

        var followers = await _dbContext.Authors
            .Where(a => a.Follows.Contains(user))
            .ToListAsync();

        foreach (var follower in followers)
        {
            follower.Follows.Remove(user);
        }

        // Delete user's cheeps and account
        var userCheeps = _dbContext.Cheeps.Where(c => c.AuthorId == userId).ToList();
        _dbContext.Cheeps.RemoveRange(userCheeps);
        _dbContext.Authors.Remove(user);

        // Save changes and sign out
        await _dbContext.SaveChangesAsync();
        await HttpContext.SignOutAsync();

        // Redirect to the logout page
        return RedirectToPage("/Account/Logout", new { area = "Identity", returnUrl = "/" });
    }
}