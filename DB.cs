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

        public static List<string> getFriends(string user)
        {
            List<string> friends = new List<string>();
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "select friends from users where email=@user";
            cmd.Parameters.Add("@user", MySqlDbType.VarChar).Value = user;
            con.Open();
            string friend ;
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {

                    friend = reader[0].ToString();
                    string[] b = friend.Split(';');
                    foreach(string f in b)
                    {
                        friends.Add(f);
                    }
                    return friends;
                }
                else
                {
                    return friends;
                }


            }
            
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
                    Messages msg = new Messages(reader["Id"].ToString(), reader["sender"].ToString(), reader["time"].ToString(), reader["content"].ToString(), reader["type"].ToString() );
                    msgs.Add(msg);

                }


            }
            return msgs;
        }



        public static void DeleteMsg(string Id)
        {
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "delete from msgs where Id=@Id";
            cmd.Parameters.Add("@Id", MySqlDbType.VarChar).Value = Id;
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


        public static void Accept(string sender, string CurrentUser, string Id)
        {
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            string q =String.Format("update users set friends= CONCAT(friends, ';{0}') where email=@currentUser", sender);
            cmd.CommandText = q;
            cmd.Parameters.Add("@currentUser", MySqlDbType.VarChar).Value = CurrentUser;
            con.Open();
            string content = sender + ", " + CurrentUser + " is now your friend :)";
            try
            {
                cmd.ExecuteScalar();
                DB.DeleteMsg(Id);
                DB.sendMessage(CurrentUser, sender, content, "FriendRequestApproval");
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
            
            
            

        }


        public static void sendMessage(string sender, string receiver, string content, string type)
        {
            MySqlConnection con = DB.Con();
            MySqlCommand cmd = new MySqlCommand();
            string msg = receiver + ", unfortunately " + sender + " denied your request ;)";
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "insert into msgs(sender, receiver, time, content, type) values(@sender, @receiver, @time, @content, @type)";
            cmd.Parameters.Add("@sender", MySqlDbType.VarChar).Value = sender;
            cmd.Parameters.Add("@receiver", MySqlDbType.VarChar).Value = receiver;
            cmd.Parameters.Add("@time", MySqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@content", MySqlDbType.VarChar).Value = content;
            cmd.Parameters.Add("@type", MySqlDbType.VarChar).Value = type;
            con.Open();
            try
            {
                cmd.ExecuteScalar();
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
        }

        public static void Deny(string receiver, string sender, string Id)
        {
            string content = receiver + " unfortunately, " + sender + " denied your friend request ;)";
            
            try
            {
                DB.sendMessage(sender, receiver, content, "FriendRequestDenial");
                DB.DeleteMsg(Id);
            }
            catch
            {

            }
        }


        

    }
}
