using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

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

        public static string getPosts(string email)
        {

            return "";
        }
    }
}
