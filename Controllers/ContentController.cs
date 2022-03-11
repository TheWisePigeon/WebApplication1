using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using MySql.Data.MySqlClient;
using System.IO;

namespace WebApplication1.Controllers
{
    public class ContentController : Controller
    {

        //app home
        public ActionResult Home(string user)
        {
            ViewBag.user = user;
            ViewBag.users = DB.getAllPosts();
            return View();
        }


        //user's posts
        public ActionResult MyPosts()
        {
            ViewBag.users = DB.getPosts(HttpContext.Session.GetString("CurrentUser"));

            return View();
        }

        //new post page view
        public ActionResult NewPost()
        {
            return View();
        }


        //make a new post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPost( PostModel post )
        {
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "insert into posts(media, description, poster) values(@media, @desc, @poster)";
            try
            {
                string desc = post.Description;
                Guid guid = Guid.NewGuid();
                string ext = Path.GetExtension(post.Media.FileName);
                string filename = post.Media.FileName;
                string uploadPath = "wwwroot/Posts/"+filename;
                string media = uploadPath;
                var stream = new FileStream(uploadPath, FileMode.Create);
                //insert
                cmd.Parameters.Add("@poster", MySqlDbType.VarChar).Value = HttpContext.Session.GetString("CurrentUser");
                cmd.Parameters.Add("@desc", MySqlDbType.VarChar).Value = desc;
                cmd.Parameters.Add("@media", MySqlDbType.VarChar).Value = media;
                con.Open();
                try
                {
                    cmd.ExecuteScalar();
                }
                catch(Exception e)
                {
                    ViewBag.error = e.Message;
                    return View();
                }
                finally
                {
                    con.Close();
                }
                post.Media.CopyToAsync(stream);
                ViewBag.users = DB.getPosts(HttpContext.Session.GetString("CurrentUser"));
                return View("MyPosts");
            }
            catch
            {

            }
            return View("MyPosts");
        }

        //delete a post
        public ActionResult Delete(string id)
        {
            
            try
            {
                DB.delete(id);
            }
            catch (Exception e)
            {
                ViewBag.error = e.Message;
                return View();
            }
            ViewBag.users = DB.getPosts(HttpContext.Session.GetString("CurrentUser"));
            return View("MyPosts");
        }

        //friends stuff
        public ActionResult Friends()
        {
            ViewBag.friends = DB.getFriends(HttpContext.Session.GetString("CurrentUser"));
            ViewBag.currentUser = HttpContext.Session.GetString("CurrentUser");
            return View();
        }

        //search for a user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchUser(IFormCollection c)
        {
            string user = c["user"].ToString();
            ViewBag.exist = DB.getUser(user);
            ViewBag.user = user;
            
            return View("Friends");
        }

        public ActionResult SendFriendRequest()
        {
             
            return View();
        }

        //send Friend request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendFriendRequest(IFormCollection c)
        {
            string receiver = c["receiver"];
            string sender = HttpContext.Session.GetString("CurrentUser");
            string content = sender + " would like to be your friend uWu";
            try
            {
                DB.sendMessage(sender, receiver, content, "FriendRequest");
            }
            catch(Exception e)
            {
                ViewBag.error = e.Message;
                return View();

            }

            ViewBag.currentUser = HttpContext.Session.GetString("CurrentUser");
            return View("Friends");
        }

        //accept request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Accept(IFormCollection c)
        {
            string sender = c["sender"];
            string Id = c["Id"];
            string currentUser = HttpContext.Session.GetString("CurrentUser");
            try
            {
                DB.Accept(sender, currentUser, Id);
            }
            catch(Exception e)
            {
                ViewBag.error = e.Message;
                return View("Error");
            }
            ViewBag.msgs = DB.getMessages(HttpContext.Session.GetString("CurrentUser"));
            return View("Messages");
        }

        //deny request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deny(IFormCollection c)
        {

            string sender = c["sender"];
            string Id = c["Id"];
            string currentUser = HttpContext.Session.GetString("CurrentUser");
            try
            {
                DB.Deny(sender, currentUser, Id);
            }
            catch (Exception e)
            {
                ViewBag.error = e.Message;
                return View("Error");
            }
            ViewBag.msgs = DB.getMessages(HttpContext.Session.GetString("CurrentUser"));
            return View("Messages");
        }

        //remove friend
        public ActionResult RemoveFriend(string user)
        {
            string current = HttpContext.Session.GetString("CurrentUser");
            string[] test = DB.RemoveFriend(user, current);
            //ViewBag.friends = DB.getFriends(HttpContext.Session.GetString("CurrentUser"));
            //ViewBag.currentUser = HttpContext.Session.GetString("CurrentUser");
            ViewBag.error = test;
            return View("Error");
        }
        //ok request
        public ActionResult Ok(IFormCollection c)
        {
            string id = c["Id"];
            try
            {
                DB.DeleteMsg(id);
            }
            catch(Exception e)
            {
                ViewBag.error = e.Message;
                return View();
            }

            ViewBag.msgs = DB.getMessages(HttpContext.Session.GetString("CurrentUser"));
            return View("Messages");
        }

        //messages stuff
        public ActionResult Messages()
        {
            ViewBag.msgs = DB.getMessages(HttpContext.Session.GetString("CurrentUser"));
            return View();
        }
    }
}
