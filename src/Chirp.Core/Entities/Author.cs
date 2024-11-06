using Microsoft.AspNetCore.Identity;
//Allows annotations like NotMapped
using System.ComponentModel.DataAnnotations.Schema;
namespace Chirp.Core.Entities
{

    public class Author : IdentityUser<int>
    {
		[NotMapped]
		//Allows for shadowing the Id attribute of IdentityUser, reference to AuthorId just returns 
		//the Id field of Identity user now. This makes it so that we do not have to refactor a 
		//billion lines of code where AuthorId is used instead of Id
        public int AuthorId {
		
		get {return Id;}
		set { Id = value;}
}

		[NotMapped]
		public string Name {
		
		get {return UserName;}
		set { UserName = value;}
}
		
		
        public string? DisplayName { get; set; }
        public string? DisplayEmail { get; set; }
        public required ICollection<Cheep> Cheeps { get; set; }
    }


}