using Chirp.Core.DTOs;
using Chirp.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{

    //SupportsGet = true is needed since BindProperty is for POST requests by default, this
    //just allows it to get bound data on a GET request as well
    [BindProperty(SupportsGet = true)]

    //author is passed by the cshtml because in it we have @page "/{author}".
    //How you name the parameter here matters, at it has to match the {paramater_name} in @page
    public string author { get; set; }
    private readonly ICheepRepository _CheepRepository;
    public required List<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository cheepRepository)
    {
        _CheepRepository = cheepRepository;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page) //author is passed by the cshtml because in it we have @page "/{author}".
                                                                //How you name the parameter here matters, at it has to match the {paramater_name} in @page
    {
        //Ensure first page is returned on invalid query for page
        if (page <= 0) page = 1;
        Cheeps = await _CheepRepository.ReadFromAuthor(page, author);
        return Page();
    }
}
