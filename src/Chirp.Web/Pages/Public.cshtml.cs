using Chirp.Core.DTOs;
using Chirp.Infrastructure.Services;
using Chirp.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using Chirp.Web.Pages.Shared;
using System.Security.Claims;


namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{

    // private readonly UserManager<ApplicationUser> _userManager;

    private readonly ICheepService _CheepService;
    private readonly ICheepRepository _CheepRepository;
    public required List<CheepDTO> Cheeps { get; set; }

    [BindProperty]
    public CheepBoxModel cheepBox { get; set; } = new CheepBoxModel();


    public PublicModel(ICheepRepository cheepRepository, ICheepService cheepService)
    {
        _CheepRepository = cheepRepository;
        _CheepService = cheepService;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        //Ensure first page is returned on invalid query value for page
        if (page <= 0) page = 1;
        Cheeps = await _CheepRepository.ReadPublicTimeline(page);
        return Page();
    }

    public async Task<ActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("CheepText", "You have exeeded max length for cheeps");
            return Page();
        }

        string? UserName = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(UserName))
        {
            throw new ArgumentNullException(nameof(UserName), "User must be authenticated.");
        }
        var CheepText = cheepBox.CheepText
            ?? throw new ArgumentNullException(nameof(cheepBox.CheepText), "CheepText cannot be null.");

        var CheepToCreate = await _CheepService.CreateCheep(UserName, cheepBox.CheepText);

        if (!string.IsNullOrEmpty(CheepToCreate.Text))
        {

            await _CheepRepository.AddCheep(CheepToCreate);
        }

        return RedirectToPage();

    }

}