using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _CheepRepository;
    public required List<CheepDTO> Cheeps { get; set; }

    public PublicModel(ICheepRepository cheepRepository)
    {
        _CheepRepository = cheepRepository;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        //Ensure first page is returned on invalid query value for page
        if (page <= 0) page = 1;
        Cheeps = await _CheepRepository.ReadPublicTimeline(page);
        return Page();
    }
}
