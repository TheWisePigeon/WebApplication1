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
        public ActionResult Home(string user)
        {
            ViewBag.user = user;
            return View();
        }


        public ActionResult MyPosts()
        {

            return View();
        }

        public ActionResult NewPost()
        {
            return View();
        }


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
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\posts", filename);
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
                ViewBag.message = media + "Success";
                return View("MyPosts");
            }
            catch
            {

            }
            return View();
        }
    }
}
