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

    // private readonly UserManager<ApplicationUser> _userManager;

    public ICheepService _CheepService;
    public ICheepRepository _CheepRepository;
    public IAuthorRepository _authorRepository;
    public required List<CheepDTO> Cheeps { get; set; }

    [BindProperty]
    public CheepBoxModel cheepBox { get; set; } = new CheepBoxModel();


    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, ICheepService cheepService)
    {
        _CheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _CheepService = cheepService;
    }


    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        //Ensure first page is returned on invalid query value for page
        if (page <= 0) page = 1;
        Cheeps = await _CheepRepository.ReadPublicTimeline(page);
        return Page();
    }

    public async Task<ActionResult> OnPost([FromQuery] int page)
    {
        if (!ModelState.IsValid)
        {
            if (page <= 0) page = 1;
            Cheeps = await _CheepRepository.ReadPublicTimeline(page);
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
            await _CheepRepository.AddCheep(CheepToCreate);
        }

        return RedirectToPage();

    }

    public async Task<ActionResult> OnPostFollow(string user, string toFollow)
    {
        await _authorRepository.Follow(user, toFollow);
        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostUnFollow(string user, string toUnfollow)
    {
        await _authorRepository.Unfollow(user, toUnfollow);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteCheep(string timestamp, string message)
    {

        var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _CheepRepository.DeleteCheeps(userid, timestamp, message);

        return RedirectToPage();

    }



}