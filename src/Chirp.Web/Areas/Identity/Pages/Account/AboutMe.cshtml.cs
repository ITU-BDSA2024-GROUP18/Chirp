using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Chirp.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Web.Areas.Identity.Pages.Account
{
    [Authorize]
    public class AboutMeModel : PageModel
    {
        private readonly ChirpDBContext _dbContext;
        private readonly IAuthorRepository _authorRepository;


        public AboutMeModel(ChirpDBContext dbContext, IAuthorRepository authorRepository)
        {
            _dbContext = dbContext;
            _authorRepository = authorRepository;
        }

        public string? UserName { get; set; }
        public string? Email { get; set; }
        public List<Cheep> UserCheeps { get; set; } = new();
        public List<string> Following { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return;

            // Get user data
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                UserName = user.UserName;
                Email = user.Email;
            }

            // Get cheeps posted by user
            UserCheeps = _dbContext.Cheeps.Where(c => c.AuthorId == userId).ToList();

            // Get following list 
            Following = await _authorRepository.GetFollowedUsers(userId);
        }

        public async Task<IActionResult> OnPostForgetMeAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return BadRequest();

            // Fetch the user from db
            var user = await _dbContext.Authors
                .Include(a => a.Follows)
                .FirstOrDefaultAsync(a => a.Id == userId);
            if (user == null) return NotFound();

            // Remove entries where the user is a follower
            if (user.Follows != null && user.Follows.Any())
            {
                _dbContext.Entry(user).Collection(u => u.Follows).Load();
                user.Follows.Clear(); // Remove all follows
            }

            // Remove entries where the user is being followed
            var followers = await _dbContext.Authors
                .Where(a => a.Follows.Contains(user))
                .ToListAsync();

            foreach (var follower in followers)
            {
                follower.Follows.Remove(user); // Remove the user from others' follows
            }

            // Delete user's cheeps
            var userCheeps = _dbContext.Cheeps.Where(c => c.AuthorId == userId).ToList();
            _dbContext.Cheeps.RemoveRange(userCheeps);

            // Delete the user account
            _dbContext.Authors.Remove(user);

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Log the user out
            await HttpContext.SignOutAsync();

            // Redirect to Public Timeline
            return RedirectToPage("/Account/Logout", new { area = "Identity", returnUrl = "/" });
        }
    }
}