using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mune.Models
{
    public class UserPost
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        [Required, MaxLength(100)]
        [DisplayName("Overskrift")]
        public string Headline { get; set; } = null!;

        [Required, MaxLength(30)]
        [DisplayName("By")]
        public string City { get; set; } = null!;

        [Required, MaxLength(500)]
        [DisplayName("Beskrivelse")]
        public string PostText { get; set; } = null!;

        [Required]
        [DisplayName("Opslået")]
        public DateTime Timestamp { get; set; }


        // Navigation property
        [ValidateNever]
        public virtual User User { get; set; } = null!;
    }
}
