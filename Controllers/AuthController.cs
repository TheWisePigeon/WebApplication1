using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(IFormCollection c)
        {
            string username = c["Username"];
            string mail = c["Email"];
            string password = c["Password"];
            string bio = c["Bio"];
            string gender = c["Gender"];

            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "insert into users(email, name, password, gender, bio, msgs, friends, pubs) values(@mail, @name, @pwd, @gender, @bio, @msgs, @friends, @pubs)";
            cmd.Parameters.Add("@mail", MySqlDbType.VarChar).Value = mail;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@pwd", MySqlDbType.VarChar).Value = password;
            cmd.Parameters.Add("@gender", MySqlDbType.VarChar).Value = gender;
            cmd.Parameters.Add("@bio", MySqlDbType.VarChar).Value = bio;
            cmd.Parameters.Add("@msgs", MySqlDbType.VarChar).Value = "";
            cmd.Parameters.Add("@friends", MySqlDbType.VarChar).Value = "";
            cmd.Parameters.Add("@pubs", MySqlDbType.VarChar).Value = "";
            try
            {
                con.Open();
                if (con.State == System.Data.ConnectionState.Open)
                {
                    cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                string ex = "Duplicate entry"+" '"+mail+"' for key 'users.PRIMARY'";
                
                if (e.Message == ex)
                {
                    ViewData["e"] = "This email is already registered, try another one";
                    return View("Register");
                }
                Console.WriteLine(e.Message);
                return View("Index");
            }
            finally
            {
                con.Close();
            }
            string[] user = { username, mail, bio, gender };
            HttpContext.Session.SetString("CurrentUser", username);

            ViewBag.user = HttpContext.Session.GetString("CurrentUser");
            return RedirectToAction("Home", "Content", new { user = HttpContext.Session.GetString("CurrentUser") });
        }
        
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(IFormCollection c)
        {
            string mail = c["Email"];
            string pwd = c["Password"];
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "select * from users where email=@mail AND password=@password";
            cmd.Parameters.Add("@mail", MySqlDbType.VarChar).Value = mail;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = pwd;
            con.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {

                    con.Close();
                    HttpContext.Session.SetString("CurrentUser", mail);
                    return RedirectToAction("Home", "Content", new { user = HttpContext.Session.GetString("CurrentUser") });
                }
                else
                {
                    con.Close();
                    ViewBag.error = "No user found, check your credentials and try again";
                    return View();
                }


            }
        }




    }
}
