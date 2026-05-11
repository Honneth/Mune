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
    public class ConversationsModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public ConversationsModel(
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
        public class InputModel{}

        public string CurrentUserId { get; set; }
        public List<Conversation> Conversations { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Finde current user
            User currentUser = await _userManager.GetUserAsync(User);
            CurrentUserId = currentUser.Id;

            // Find currents users conversations 
             Conversations = await _context.Conversations
                .Where(c =>
                    c.User1Id == currentUser.Id ||
                    c.User2Id == currentUser.Id
                )
                .ToListAsync();

            foreach (Conversation c in Conversations)
            {
                // If current user is 1 - no problem
                if (c.User1Id == CurrentUserId)
                {
                    // Get username sender
                    User senderUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == c.User2Id);
                        c.User2Id = senderUser.UserName;

                // If current user is 2 - swap users
                } else if (c.User2Id == CurrentUserId)
                {

                    string oldUser1Id = c.User1Id;

                    // Move user 2 to 1, so user 1 is current user
                    c.User1Id = c.User2Id;

                    // Get username for old user one and save as user2 username.
                    User senderUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == oldUser1Id);
                        c.User2Id = senderUser.UserName;
                }
            }

            return Page();
        }
    }
}
