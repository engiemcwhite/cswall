using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using wall.Models;

namespace wall.Controllers
{
    public class WallController : Controller
    {
        private readonly DbConnector _dbConnector;
 
        public WallController(DbConnector connect)
        {
            _dbConnector = connect;
        }
 
        [HttpGet]
        [Route("/wall")]
        public IActionResult Index()
        {
            int? idnum = HttpContext.Session.GetInt32("id");
            if(idnum == null){
                return Redirect("/");
            }
            string query = "SELECT * FROM users WHERE userid = "+idnum;
            List<Dictionary<string, object>> MyUser = _dbConnector.Query(query);
            foreach(var user in MyUser){
                ViewBag.Welcome = "Welcome, "+user["firstname"]+"!";
            }
            ViewBag.Id = idnum;
            ViewBag.Now = DateTime.Now;
            ViewBag.Thirty = TimeSpan.FromMinutes(30);
            ViewBag.Posts = _dbConnector.Query("SELECT * FROM users JOIN posts ON users.userid = posts.user_id");
            ViewBag.Comments = _dbConnector.Query("SELECT * FROM users JOIN comments ON users.userid = comments.user_id");
            return View();
            // Other code
        }

        [HttpPost]
        [Route("/wall/post")]
        public IActionResult Post(string posts){
            int idnum = (int)HttpContext.Session.GetInt32("id");
            if(posts.Length == 0){
                return RedirectToAction("Index");
            }
            string query = "INSERT INTO posts (content, user_id, created_at, updated_at) VALUES ('"+posts+"','"+idnum+"',NOW(),NOW())";
            _dbConnector.Query(query);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/wall/comment")]
        public IActionResult Comment(string comment, int postid){
            int idnum = (int)HttpContext.Session.GetInt32("id");
            if(comment.Length == 0){
                return RedirectToAction("Index");
            }
            string query = "INSERT INTO comments (reply, user_id, post_id, created_at, updated_at) VALUES ('"+comment+"','"+idnum+"','"+postid+"',NOW(),NOW())";
            _dbConnector.Query(query);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/wall/delete")]
        public IActionResult Delete(string texttype, int theid){
            if(texttype == "posts"){
                string query0 = "DELETE FROM comments WHERE post_id ="+theid;
                string query1 = "DELETE FROM posts WHERE id = "+theid;
                _dbConnector.Query(query0);
                _dbConnector.Query(query1);
            }
            else{
                string query2 = "DELETE FROM comments WHERE id ="+theid;
                _dbConnector.Query(query2);
            }
            
            return RedirectToAction("Index");
        }
        

    }
}