using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mune.Models
{
    public class Message
    {
        public int Id { get; set; }


        // Foreign keys
        [ForeignKey(nameof(Conversation))]
        public int ConversationId { get; set; }


        [ForeignKey(nameof(Sender))]
        public string SenderId { get; set; } = null!;


        [ForeignKey(nameof(Receiver))]
        public string ReceiverId { get; set; } = null!;


        // Details

        [Required, MaxLength(500)]
        [DisplayName("Besked")]
        public string MessageText { get; set; }

        [Required]
        [DisplayName("Opslået")]
        public DateTime Timestamp { get; set; }


        // Relationships
        [ValidateNever]
        public virtual Conversation Conversation { get; set; } = null!;

        [ValidateNever]
        public virtual User Sender { get; set; } = null!;

        [ValidateNever]
        public virtual User Receiver { get; set; } = null!;
    }
}
