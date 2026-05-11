// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mune.Data;
using Mune.Models;
using NuGet.Protocol.Plugins;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Mune.Areas.Identity.Pages.Account.Manage
{
    public class ConversationModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public ConversationModel(
            UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            // We send new messages from this page, so we need:
            [Required]
            [Display(Name = "Besked")]
            public string Message { get; set; }
        }

        // Passed from Conversations.cshtml on first load of page
        [BindProperty(SupportsGet = true)]
        public string CurrentUserUserName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int ConversationId { get; set; } // Sender2Id = sender userName


        // ... also passed on loads of page from itself
        public string reciverUserName { get; set; }

        // Local
        public ICollection<Mune.Models.Message> Messages;

        // On load page usernames are needed and messages
        public async Task<IActionResult> OnGetAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Finde current user
            User currentUser = await _userManager.GetUserAsync(User);

            CurrentUserUserName = currentUser.UserName;

            // Find currents users conversations 
            Conversation conversation = await _context.Conversations
                   .Include(c => c.Messages)
                        .ThenInclude(m => m.Sender)
                    .Include(c => c.Messages)
                        .ThenInclude(m => m.Receiver)
               .FirstOrDefaultAsync(c => c.Id == ConversationId);

            // Get all messages from conversation
            Messages = conversation.Messages;
            List<Mune.Models.Message> messagesList = conversation.Messages.ToList();
            reciverUserName = messagesList[0].Receiver.UserName;

            return Page();
        }

        // On post conversation is needed, users and we compose the message
        public async Task<IActionResult> OnPostAsync()
        {

            // Find currents users conversations 
            Conversation conversation = await _context.Conversations
                   .Include(c => c.Messages)
                        .ThenInclude(m => m.Sender)
                    .Include(c => c.Messages)
                        .ThenInclude(m => m.Receiver)
               .FirstOrDefaultAsync(c => c.Id == ConversationId);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            List<Mune.Models.Message> messagesList = conversation.Messages.ToList();

            // Here sender/reciever is reversed compared to when handling message functionality for getting messages
            User sender = messagesList[0].Sender;
            User receiver = messagesList[0].Receiver;

            if (sender == null)
            {
                ModelState.AddModelError("Input.Receiver", "Modtager eksisterer ikke.");
                return Page();
            }

            if (sender.Id == receiver.Id)
            {
                ModelState.AddModelError("Input.Receiver", "Du kan ikke sende beskeder til dig selv :(");
                return Page();
            }

            // Generate message object
            var message = new Mune.Models.Message
            {
                ReceiverId = sender.Id,
                SenderId = receiver.Id,
                MessageText = Input.Message,
                Timestamp = DateTime.UtcNow, // This message
                Conversation = conversation
            };

            _context.Messages.Add(message);

            await _context.SaveChangesAsync();

            StatusMessage = "Message sent.";

            // Preserve ConversationID across multiple loads
            return RedirectToPage(new {ConversationId});
        }
    }
}
