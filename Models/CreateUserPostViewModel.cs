using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mune.Models
{
    public class CreateUserPostViewModel
    {

        // Attributes are for FORM validation
        [Required, MaxLength(100)]
        public string Headline { get; set; } = null!;

        [Required, MaxLength(30)]
        public string City { get; set; } = null!;

        [Required, MaxLength(500)]
        public string PostText { get; set; } = null!;
    }
}
