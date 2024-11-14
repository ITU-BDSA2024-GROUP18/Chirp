using Chirp.Core.DTOs;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Web.Pages.Shared;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{

    //SupportsGet = true is needed since BindProperty is for POST requests by default, this
    //just allows it to get bound data on a GET request as well
    [BindProperty(SupportsGet = true)]

    //author is passed by the cshtml because in it we have @page "/{author}".
    //How you name the parameter here matters, at it has to match the {paramater_name} in @page
    public string author { get; set; }

    [BindProperty]
    public CheepBoxModel cheepBox { get; set; } = new CheepBoxModel();
    private readonly ICheepRepository _CheepRepository;

    private readonly ICheepService _CheepService;
    public required List<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository cheepRepository, ICheepService cheepService)
    {
        _CheepRepository = cheepRepository;
        _CheepService = cheepService;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page) //author is passed by the cshtml because in it we have @page "/{author}".
                                                                //How you name the parameter here matters, at it has to match the {paramater_name} in @page
    {
        //Ensure first page is returned on invalid query for page
        if (page <= 0) page = 1;
        Cheeps = await _CheepRepository.ReadFromAuthor(page, author);
        return Page();
    }

    public async Task<ActionResult> OnPost()
    {
        var UserName = await _CheepRepository.GetAuthorByName(User.Identity.Name);

        var CheepToCreate = await _CheepService.CreateCheep(UserName.Id, cheepBox.CheepText);

        if (!string.IsNullOrEmpty(cheepBox.CheepText))
        {

            await _CheepRepository.AddCheep(CheepToCreate);
        }

        return RedirectToPage();

    }
}
