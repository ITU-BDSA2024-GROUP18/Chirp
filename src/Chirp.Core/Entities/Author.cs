using Microsoft.AspNetCore.Identity;
// Allows annotations like NotMapped
using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Entities
{
    /// <summary>
    /// Represents an author entity with identity and additional properties.
    /// </summary>
    public class Author : IdentityUser
    {
        /// <summary>
        /// Collection of Cheeps created by the author.
        /// </summary>
        public required ICollection<Cheep> Cheeps { get; set; }

        /// <summary>
        /// List of authors followed by this author.
        /// </summary>
        public ICollection<Author> Follows { get; set; } = new List<Author>();

        /// <summary>
        /// Optional username provided by the user during account creation.
        /// </summary>
        public string? UserCreatedUserName { get; set; }
    }
}