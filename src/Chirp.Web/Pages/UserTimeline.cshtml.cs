using Chirp.Core.DTOs;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Web.Pages.Shared;
using System.Security.Claims;

namespace Chirp.Web.Pages;

/// <summary>
/// PageModel for the User Timeline page, managing user-specific Cheeps and interactions.
/// </summary>
public class UserTimelineModel(ICheepService cheepService, IAuthorService authorService) : PageModel
{
    /// <summary>
    /// The username of the author whose timeline is being viewed.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public required string Author { get; set; }

    /// <summary>
    /// Bound property for creating a new Cheep.
    /// </summary>
    [BindProperty]
    public CheepBoxModel cheepBox { get; set; } = new CheepBoxModel();

    /// <summary>
    /// Service for Cheep operations.
    /// </summary>
    public ICheepService _CheepService { get; } = cheepService;

    /// <summary>
    /// Service for Author operations.
    /// </summary>
    public IAuthorService _AuthorService { get; } = authorService;

    /// <summary>
    /// List of Cheeps displayed on the user's timeline.
    /// </summary>
    public required List<CheepDTO> Cheeps { get; set; }

    /// <summary>
    /// Handles GET requests to fetch a paginated list of Cheeps for the user's timeline.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <returns>An ActionResult rendering the page.</returns>
    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        if (page <= 0) page = 1; // Default to the first page on invalid input
        Cheeps = await _CheepService.GetCheepsFromFollows(page, Author);
        return Page();
    }

    /// <summary>
    /// Handles POST requests to create a new Cheep.
    /// </summary>
    /// <param name="page">The page number for pagination.</param>
    /// <returns>An ActionResult rendering the page.</returns>
    public async Task<ActionResult> OnPost([FromQuery] int page)
    {
        if (!ModelState.IsValid)
        {
            if (page <= 0) page = 1;
            Cheeps = await _CheepService.GetCheepsFromFollows(page, Author);
            return Page();
        }

        string? userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userName))
        {
            throw new ArgumentNullException(nameof(userName), "User must be authenticated.");
        }

        var cheepText = cheepBox.CheepText ?? throw new ArgumentNullException(nameof(cheepBox.CheepText), "CheepText cannot be null.");

        var cheepToCreate = await _CheepService.CreateCheep(userName, cheepText);

        if (!string.IsNullOrEmpty(cheepText))
        {
            await _CheepService.AddCheep(cheepToCreate);
        }

        return RedirectToPage();
    }

    /// <summary>
    /// Handles POST requests to follow another user.
    /// </summary>
    /// <param name="user">The username of the current user.</param>
    /// <param name="toFollow">The username of the user to follow.</param>
    /// <returns>An ActionResult redirecting to the page.</returns>
    public async Task<ActionResult> OnPostFollow(string user, string toFollow)
    {
        await _AuthorService.Follow(user, toFollow);
        return RedirectToPage();
    }

    /// <summary>
    /// Handles POST requests to unfollow a user.
    /// </summary>
    /// <param name="user">The username of the current user.</param>
    /// <param name="toUnfollow">The username of the user to unfollow.</param>
    /// <returns>An ActionResult redirecting to the page.</returns>
    public async Task<ActionResult> OnPostUnFollow(string user, string toUnfollow)
    {
        await _AuthorService.Unfollow(user, toUnfollow);
        return RedirectToPage();
    }

    /// <summary>
    /// Handles POST requests to delete a Cheep.
    /// </summary>
    /// <param name="timestamp">The timestamp of the Cheep to delete.</param>
    /// <param name="message">The content of the Cheep to delete.</param>
    /// <returns>An IActionResult redirecting to the page.</returns>
    public async Task<IActionResult> OnPostDeleteCheep(string timestamp, string message)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _CheepService.DeleteCheep(userId, timestamp, message);

        return RedirectToPage();
    }
}