using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(IFormCollection c)
        {
            string name = c["name"];
            string pwd = c["pwd"];
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "select * from admins where name=@name AND pwd=@pwd";
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@pwd", MySqlDbType.VarChar).Value = pwd;
            con.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {

                    con.Close();
                    HttpContext.Session.SetString("CurrentUser", name);
                    ViewBag.users = DB.getUsers();
                    ViewBag.posts = DB.getAllPosts();
                    return View("AdminPanel");
                }
                else
                {
                    con.Close();
                    ViewBag.error = "No user found, check your credentials and try again";
                    return View();
                }


            }
        }
        public ActionResult AdminPanel()
        {
            ViewBag.users = DB.getUsers();
            ViewBag.posts = DB.getAllPosts();
            return View();
        }

        public ActionResult DeletePost(string id)
        {
            DB.delete(id);
            ViewBag.users = DB.getUsers();
            ViewBag.posts = DB.getAllPosts();
            return View("AdminPanel");
        }

        public ActionResult DeleteAccount(string user)
        {
            DB.DeleteAccount(user);
            ViewBag.users = DB.getUsers();
            ViewBag.posts = DB.getAllPosts();
            return View("AdminPanel");
        }

        public ActionResult ResetAccount(string user)
        {
            DB.ResetAccount(user);
            ViewBag.users = DB.getUsers();
            ViewBag.posts = DB.getAllPosts();
            return View("AdminPanel");
        }
    }
}
