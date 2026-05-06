using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mune.Data;
using Mune.Models;

namespace Mune.Controllers
{
    public class UserPostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserPosts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserPosts.Include(u => u.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPost = await _context.UserPosts
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPost == null)
            {
                return NotFound();
            }

            return View(userPost);
        }

        // GET: UserPosts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: UserPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Headline,City,PostText,Timestamp")] UserPost userPost)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userPost.UserId);
            return View(userPost);
        }

        // GET: UserPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPost = await _context.UserPosts.FindAsync(id);
            if (userPost == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userPost.UserId);
            return View(userPost);
        }

        // POST: UserPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Headline,City,PostText,Timestamp")] UserPost userPost)
        {
            if (id != userPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserPostExists(userPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userPost.UserId);
            return View(userPost);
        }

        // GET: UserPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPost = await _context.UserPosts
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPost == null)
            {
                return NotFound();
            }

            return View(userPost);
        }

        // POST: UserPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userPost = await _context.UserPosts.FindAsync(id);
            if (userPost != null)
            {
                _context.UserPosts.Remove(userPost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserPostExists(int id)
        {
            return _context.UserPosts.Any(e => e.Id == id);
        }
    }
}
