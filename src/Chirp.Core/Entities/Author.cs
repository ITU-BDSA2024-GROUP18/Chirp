using Microsoft.AspNetCore.Identity;
//Allows annotations like NotMapped
using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Entities

{

    public class Author : IdentityUser
    {
        public required ICollection<Cheep> Cheeps { get; set; }
        public ICollection<Author> Follows { get; set; } = new List<Author>();

        public string? UserCreatedUserName { get; set; }

    }


}