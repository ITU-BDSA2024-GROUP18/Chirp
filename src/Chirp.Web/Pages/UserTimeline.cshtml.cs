using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _CheepRepository;
    public required List<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository cheepRepository)
    {
        _CheepRepository = cheepRepository;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page, string author)
    {
        //Ensure first page is returned on invalid query for page
        if (page <= 0) page = 1;
        Cheeps = await _CheepRepository.ReadFromAuthor(page, author);
        return Page();
    }
}
