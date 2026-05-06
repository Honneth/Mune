using System.ComponentModel.DataAnnotations;

namespace Mune.Models
{
    public class EditProfileViewModel
    {

        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Instrument")]
        public string? Instrument { get; set; }

        [Display(Name = "By")]
        public string? City { get; set; }

        [Display(Name = "Navn")]
        public string? Name { get; set; }

    }
}
