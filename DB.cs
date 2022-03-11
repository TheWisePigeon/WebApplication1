using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using WebApplication1.Models;

namespace WebApplication1
{
    public class DB
    {

        public static MySqlConnection Con()
        {
            string pass = "";
            MySqlConnection con = new MySqlConnection("Data Source=localhost;" + "Initial Catalog=atlas;" + "User ID=root;" + "Password=" + pass + ";");
            return  con;
        }

        public static List<Post> getPosts(string email)
        {
            List<Post> posts = new List<Post>();
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "select * from posts where poster=@mail order by Id desc";
            cmd.Parameters.Add("@mail", MySqlDbType.VarChar).Value = email;
            con.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Post post = new Post(reader["Id"].ToString(), reader["media"].ToString(), reader["description"].ToString(), email);
                    posts.Add(post);

                }


            }
            return posts;
        }

        public static List<Post> getAllPosts()
        {
            List<Post> posts = new List<Post>();
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "select * from posts order by Id desc";
            
            con.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Post post = new Post(reader["Id"].ToString(), reader["media"].ToString(), reader["description"].ToString(), reader["poster"].ToString());
                    posts.Add(post);

                }


            }
            return posts;
        }

        public static void delete(string id)
        {
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "delete from posts where Id=@id";
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = id;
            con.Open();
            try
            {
                cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                
            }
            finally
            {
                con.Close();
            }

        }

        public static Boolean getUser(string mail)
        {
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            string foundUser;
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "select * from users where email=@mail";
            cmd.Parameters.Add("@mail", MySqlDbType.VarChar).Value = mail;
            con.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    foundUser = reader["email"].ToString();
                    con.Close();
                    return true;
                }
                else
                {
                    con.Close();
                    return false;
                }


            }
            
        }

        public static List<Friend> friends(string user)
        {
            List<Friend> friends = new List<Friend>();

            return friends;
        }


        public static List<Messages> getMessages(string user)
        {
            List<Messages> msgs = new List<Messages>();
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "select * from msgs where receiver=@receiver order by Id desc";
            cmd.Parameters.Add("@receiver", MySqlDbType.VarChar).Value = user;
            con.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Messages msg = new Messages(reader["sender"].ToString(), reader["time"].ToString(), reader["content"].ToString(), reader["type"].ToString() );
                    msgs.Add(msg);

                }


            }
            return msgs;
        }


        public static void Accept(string user)
        {

        }

    }
}
