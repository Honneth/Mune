// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mune.Data;
using Mune.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Mune.Areas.Identity.Pages.Account.Manage
{
    public class NewMessage : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public NewMessage(
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
            [Required]
            [Display(Name = "Besked")]
            public string Message { get; set; }

            [Required]
            [Display(Name = "Modtager")]
            public string Receiver { get; set; }
        }

        // New Conversation with message
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var sender = await _userManager.GetUserAsync(User);
            var receiver = await _userManager.FindByNameAsync(Input.Receiver);

            if (receiver == null)
            {
                ModelState.AddModelError("Input.Receiver", "Modtager eksisterer ikke.");
                return Page();
            }

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == sender.Id     && c.User2Id == receiver.Id) ||
                    (c.User1Id == receiver.Id   && c.User2Id == sender.Id)
                );


            if (conversation == null)
            {
                conversation = new Conversation
                {
                    // Conversation start
                    Timestamp = DateTime.UtcNow,
                    User1Id = sender.Id,
                    User2Id = receiver.Id
                };

                _context.Conversations.Add(conversation);
            }

            var message = new Message
            {
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                MessageText = Input.Message,
                Timestamp = DateTime.UtcNow, // This message
                Conversation = conversation
            };

            _context.Messages.Add(message);

            await _context.SaveChangesAsync();

            StatusMessage = "Message sent.";

            return RedirectToPage();
        }
    }
}
