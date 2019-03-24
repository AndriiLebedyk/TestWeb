using System;
using System.ComponentModel.DataAnnotations;

namespace TestWebNotCore.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Post text required")]
        public string Body { get; set; }
        public string AuthorName { get; set; }
        public DateTime Date { get; set; }
    }
}