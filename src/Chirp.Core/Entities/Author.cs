using Microsoft.AspNetCore.Identity;
//Allows annotations like NotMapped
using System.ComponentModel.DataAnnotations.Schema;
namespace Chirp.Core.Entities
{

    public class Author : IdentityUser
    {
        public required ICollection<Cheep> Cheeps { get; set; }
        //public ICollection<Author>? UsersFollowed { get; set; }
    }


}