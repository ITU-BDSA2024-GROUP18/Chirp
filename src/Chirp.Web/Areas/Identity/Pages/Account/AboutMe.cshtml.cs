using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Chirp.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace Chirp.Web.Areas.Identity.Pages.Account
{
    [Authorize]
    public class AboutMeModel : PageModel
    {
        private readonly ChirpDBContext _dbContext;

        public AboutMeModel(ChirpDBContext dbContext)
        {
            _dbContext = dbContext;
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

            // TODO: Get following list 
        }

        public async Task<IActionResult> OnPostForgetMeAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return BadRequest();

            // Fetch the user from the database
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // Delete user's cheeps
            var userCheeps = _dbContext.Cheeps.Where(c => c.AuthorId == userId).ToList();
            _dbContext.Cheeps.RemoveRange(userCheeps);

            // Delete the user account
            _dbContext.Users.Remove(user);

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Log the user out
            await HttpContext.SignOutAsync();

            // Redirect to Public Timeline
            return RedirectToPage("/Account/Logout", new { area = "Identity", returnUrl = "/Public" });
        }
    }
}