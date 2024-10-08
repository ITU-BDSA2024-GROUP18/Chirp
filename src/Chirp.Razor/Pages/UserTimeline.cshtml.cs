using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet([FromQuery] int page, string author)
    {   
        //Ensure first page is returned on invalid query for page
        if (page <= 0) page = 1;
        Cheeps = _service.GetCheepsFromAuthor(page, author);
        return Page();
    }
}
