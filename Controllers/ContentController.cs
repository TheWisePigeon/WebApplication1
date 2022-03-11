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

        //send Friend request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendFriendRequest(string sender, string receiver)
        {
            string msg = "Hey "+ receiver + "! " + sender + " would like to become your friend :)";
            return View();
        }
    }
}
