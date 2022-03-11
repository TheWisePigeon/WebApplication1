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
            string msg = "Hey "+ receiver + "! " + sender + " would like to become your friend :)";
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text; 
            cmd.CommandText = "insert into msgs(sender, receiver, time, content, type) values(@sender, @receiver, @time, @content, @type)";
            cmd.Parameters.Add("@sender", MySqlDbType.VarChar).Value = sender;
            cmd.Parameters.Add("@receiver", MySqlDbType.VarChar).Value = receiver;
            cmd.Parameters.Add("@time", MySqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@content", MySqlDbType.VarChar).Value = msg;
            cmd.Parameters.Add("@type", MySqlDbType.VarChar).Value = "FriendRequest";
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

            ViewBag.currentUser = HttpContext.Session.GetString("CurrentUser");
            return View("Friends");
        }

        //accept request
        public ActionResult Accept()
        {

            return View();
        }

        //deny request
        public ActionResult Deny()
        {
            return View();
        }

        //messages stuff
        public ActionResult Messages()
        {
            ViewBag.msgs = DB.getMessages(HttpContext.Session.GetString("CurrentUser"));
            return View();
        }
    }
}
