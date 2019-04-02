using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TestWebNotCore.Models;
using System.Data.Entity;

namespace TestWebNotCore.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext m_db = new ApplicationDbContext();

        [AllowAnonymous]
        public ActionResult Index() {
            using(var m_db = new ApplicationDbContext())
            {
                var lastMonth = DateTime.Now.AddMonths(-1);
                return View(m_db.Posts.Where(x => x.Date >= lastMonth).ToList());
            }
        }

        public ActionResult Details(int? id)
        {
            using (var m_db = new ApplicationDbContext())
            {
                if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var posts = m_db.Posts.Include(x => x.PostComments).ToList();
                Post post = posts.Find(x => x.Id == id);
                return post == null ? (ActionResult)HttpNotFound() : View(post);
            }
        }

        public ActionResult Create() => View();

        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Title,Body")] Post post)
        {
            if (!ModelState.IsValid) return View(post);
            using (var m_db = new ApplicationDbContext())
            {
                post.AuthorName = User.Identity.Name;
                post.Date = DateTime.Now;
                m_db.Posts.Add(post);
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using(var m_db = new ApplicationDbContext())
            {
                Post post = m_db.Posts.Find(id);
                m_db.Posts.Remove(post);
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult AddComment(string commentText, int postId)
        {
            using(var m_db = new ApplicationDbContext())
            {
                var comment = new Comment
                {
                    Author = User.Identity.Name.Substring(0, User.Identity.Name.IndexOf('@')),
                    Text = commentText,
                    Post = m_db.Posts.Find(postId),
                    PostId = postId
                };
                m_db.Comments.Add(comment);
                m_db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = postId });
        }

        [HttpPost]
        public ActionResult DeleteComment(int id)
        {
            using(var m_db = new ApplicationDbContext())
            {
                var comment = m_db.Comments.Find(id);
                var postId = comment.PostId;
                m_db.Comments.Remove(comment);
                m_db.SaveChanges();
                return RedirectToAction("Details", new { id = postId });
            }
        }
    }
}