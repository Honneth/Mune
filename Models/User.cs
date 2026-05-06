using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mune.Models
{
    // Model der repræsenterer brugere
    // Bygger på Identity user, da der er authentication
    public class User : IdentityUser
    {
        //  Key og PhoneNumber nedarves fra IdentityUse

        public String? Name { get; set; }


        public String? Instrument { get; set; }


        public String? City { get; set; }

        // Navigation property
        public virtual ICollection<UserPost>? UserPosts { get; set; }
        // Virtual: Entity Framework laver IKKE nogen kolonne i Customers table
    }
}
