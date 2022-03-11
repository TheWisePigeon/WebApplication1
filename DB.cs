using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;

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
    }
}
