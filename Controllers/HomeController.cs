using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mune.Data;
using Mune.Models;
using System.Diagnostics;

namespace Mune.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Individual pages for each post (with deatails about post)
        public async Task<IActionResult> PostDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPost = await _context.UserPosts
                .Include(p => p.User) // For message funcionality to be added later
                .FirstOrDefaultAsync(m => m.Id == id);

            if (userPost == null)
            {
                return NotFound();
            }

            return View(userPost);
        }

        // Load homescreen feed data (posts)
        public IActionResult Index()
        {
            var posts = _context.UserPosts
            .Include(p => p.User)
            .OrderByDescending(p => p.Timestamp)
            .ToList();

            return View(posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
