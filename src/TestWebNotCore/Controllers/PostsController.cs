using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TestWebNotCore.Models;

namespace TestWebNotCore.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext m_db = new ApplicationDbContext();

        public ActionResult Index() => View(m_db.Posts.ToList());

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Post post = m_db.Posts.Find(id);
            return post == null ? (ActionResult) HttpNotFound() : View(post);
        }

        public ActionResult Create() => View();

        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Title,Body")] Post post)
        {
            if (!ModelState.IsValid) return View(post);

            post.AuthorName = User.Identity.Name;
            post.Date = DateTime.Now;
            m_db.Posts.Add(post);
            m_db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Post post = m_db.Posts.Find(id);
            m_db.Posts.Remove(post);
            m_db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}