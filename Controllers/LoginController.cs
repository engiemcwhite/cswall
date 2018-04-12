using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using wall.Models;

namespace wall.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbConnector _dbConnector;
 
        public LoginController(DbConnector connect)
        {
            _dbConnector = connect;
        }
 
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
            // Other code
        }

        [HttpPost]
        [Route("/register")]
        public IActionResult Register(User model)
        {
            if(ModelState.IsValid){
                string query = "SELECT * FROM users WHERE email = '"+model.Email+"'";
                List<Dictionary<string, object>> emailusers = _dbConnector.Query(query);
                if(emailusers.Count > 0){
                    ViewBag.Test = 1;
                    ViewBag.Error2 = "An account with that email address already exists!";
                    return View("Index");
                }
                string querystring = "INSERT INTO Users (firstname, lastname, email, password, createdat, updatedat) VALUES ('"+model.FirstName+"','"+model.LastName+"','"+model.Email+"','"+model.Password+"',NOW(),NOW())";
                _dbConnector.Query(querystring);
                string query2 = "SELECT userid FROM users WHERE email = '"+model.Email+"'";
                List<Dictionary<string,object>> userid = _dbConnector.Query(query2);
                foreach(var idnum in userid){
                    HttpContext.Session.SetInt32("id",(int)idnum["userid"]);
                } 
                return Redirect("/wall");
            }
            return View("Index");
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Login(string email, string password){
            List<Dictionary<string, object>> MyUser = _dbConnector.Query("SELECT * FROM users WHERE email = '"+email+"' AND password = '"+password+"'");
            if(MyUser.Count == 0){
                ViewBag.Error = "Invalid login information!";
                ViewBag.TestCase = 2; 
                return View("Index");
            }
            else{
                foreach(var auser in MyUser){
                    HttpContext.Session.SetInt32("id",(int)auser["userid"]);
                }
                return Redirect("/wall");
            }
        }

        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}