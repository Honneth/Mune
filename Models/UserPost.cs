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
        public string Headline { get; set; } = null!;

        [Required, MaxLength(30)]
        public string City { get; set; } = null!;

        [Required, MaxLength(500)]
        public string PostText { get; set; } = null!;

        [Required]
        public DateTime? Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual User? User { get; set; } = null!;
        // Virtual: Entity Framework laver IKKE nogen kolonne i Customers table
    }
}
