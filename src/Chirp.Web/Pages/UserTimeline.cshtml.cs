﻿using Chirp.Core.DTOs;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Web.Pages.Shared;
using System.Security.Claims;

namespace Chirp.Web.Pages;

public class UserTimelineModel(ICheepRepository cheepRepository, ICheepService cheepService, IAuthorRepository authorRepository, IAuthorService authorService) : PageModel
{

    //SupportsGet = true is needed since BindProperty is for POST requests by default, this
    //just allows it to get bound data on a GET request as well
    [BindProperty(SupportsGet = true)]

    //author is passed by the cshtml because in it we have @page "/{author}".
    //How you name the parameter here matters, at it has to match the {paramater_name} in @page
    public required string Author { get; set; }

    [BindProperty]
    public CheepBoxModel cheepBox { get; set; } = new CheepBoxModel();
    public ICheepRepository _CheepRepository = cheepRepository;
    public ICheepService _CheepService = cheepService;
    public IAuthorRepository _authorRepository = authorRepository;
    public IAuthorService _authorService = authorService;
    public required List<CheepDTO> Cheeps { get; set; }

    public async Task<ActionResult> OnGet([FromQuery] int page) //author is passed by the cshtml because in it we have @page "/{author}".
                                                                //How you name the parameter here matters, at it has to match the {paramater_name} in @page
    {
        //Ensure first page is returned on invalid query for page
        if (page <= 0) page = 1;
        Cheeps = await _CheepRepository.ReadFromFollows(page, Author);
        return Page();
    }

    public async Task<ActionResult> OnPost([FromQuery] int page)
    {
        if (!ModelState.IsValid)
        {
            if (page <= 0) page = 1;
            Cheeps = await _CheepRepository.ReadFromFollows(page, Author);
            return Page();
        }

        string? UserName = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(UserName))
        {
            throw new ArgumentNullException(nameof(UserName), "User must be authenticated.");
        }
        var CheepText = cheepBox.CheepText
            ?? throw new ArgumentNullException(nameof(cheepBox.CheepText), "CheepText cannot be null.");

        var CheepToCreate = await _CheepService.CreateCheep(UserName, CheepText);

        if (!string.IsNullOrEmpty(CheepText))
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