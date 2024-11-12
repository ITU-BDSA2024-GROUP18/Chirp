using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core.Repositories;
using Chirp.Core.DTOs;
using Chirp.Core.Entities;

namespace Chirp.Web.Pages;

public class CheepBoxModel : PageModel
{

    [BindProperty]
    public required Cheep CheepMessage { get; set; }

    private readonly ICheepRepository _CheepRepository;

    public required List<CheepDTO> Cheeps { get; set; }


    public CheepBoxModel(ICheepRepository cheepRepository)
    {

        _CheepRepository = cheepRepository;
    }

    public async Task<ActionResult> OnPost([FromQuery] int page)
    {

        await _CheepRepository.AddCheep(CheepMessage);

        return Redirect("/");

    }

}