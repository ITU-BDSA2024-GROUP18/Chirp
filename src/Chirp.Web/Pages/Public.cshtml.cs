using Chirp.Core.DTOs;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using Chirp.Web.Pages.Shared;
using System.Security.Claims;
using Chirp.Infrastructure.Repositories;


namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{

    public ICheepService _CheepService;
    public IAuthorService _AuthorService;
    public required List<CheepDTO> Cheeps { get; set; }

    [BindProperty]
    public CheepBoxModel cheepBox { get; set; } = new CheepBoxModel();


    public PublicModel(ICheepService cheepService, IAuthorService authorService)
    {
        _AuthorService = authorService;
        _CheepService = cheepService;
    }


    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        //Ensure first page is returned on invalid query value for page
        if (page <= 0) page = 1;
        Cheeps = await _CheepService.GetCheeps(page);
        return Page();
    }

    public async Task<ActionResult> OnPost([FromQuery] int page)
    {
        if (!ModelState.IsValid)
        {
            if (page <= 0) page = 1;
            Cheeps = await _CheepService.GetCheeps(page);
            return Page();
        }

        string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(UserId))
        {
            throw new ArgumentNullException(nameof(UserId), "User must be authenticated.");
        }
        var CheepText = cheepBox.CheepText
            ?? throw new ArgumentNullException(nameof(cheepBox.CheepText), "CheepText cannot be null.");

        var CheepToCreate = await _CheepService.CreateCheep(UserId, cheepBox.CheepText);

        if (!string.IsNullOrEmpty(CheepToCreate.Text))
        {
            await _CheepService.AddCheep(CheepToCreate);
        }

        return RedirectToPage();

    }

    public async Task<ActionResult> OnPostFollow(string user, string toFollow)
    {
        await _AuthorService.Follow(user, toFollow);
        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostUnFollow(string user, string toUnfollow)
    {
        await _AuthorService.Unfollow(user, toUnfollow);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteCheep(string timestamp, string message)
    {

        var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _CheepService.DeleteCheep(userid, timestamp, message);

        return RedirectToPage();

    }



}