using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mune.Models
{
    public class User : IdentityUser
    {
        //  Key and Phone is inherited from IdentityUser

        public String? Name { get; set; }


        public String? Instrument { get; set; }


        public String? City { get; set; }

        // Navigation properties
        public virtual ICollection<UserPost>? UserPosts { get; set; }

        public virtual ICollection<Message> SentMessages { get; set; }

        public virtual ICollection<Message> ReceivedMessages { get; set; }

    }
}
