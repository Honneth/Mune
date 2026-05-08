using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mune.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Startidspunkt")]
        public DateTime Timestamp { get; set; }

        public string User1Id { get; set; }
        public string User2Id { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual ICollection<Message>? Messages { get; set; }

    }
}
