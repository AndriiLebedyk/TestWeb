using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TestWeb.Data;
using TestWeb.Models;

namespace TestWeb.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext m_context;

        public PostsController(ApplicationDbContext context) => m_context = context;

        public async Task<IActionResult> Index() => View(await m_context.Post.ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await m_context.Post
                .SingleOrDefaultAsync(m => m.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Title,Body")] Post post)
        {
            if (!ModelState.IsValid) return View(post);

            post.AuthorName = User.Identity.Name;
            post.Date = DateTime.Now;
            m_context.Add(post);
            await m_context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await m_context.Post.SingleOrDefaultAsync(m => m.Id == id);
            m_context.Post.Remove(post);
            return RedirectToAction(nameof(Index));
        }
    }
}