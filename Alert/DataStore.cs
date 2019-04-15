using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Alert
{
    class DataStore
    {
        static string DBName = "alert.db";
        static string DBPath = "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + @DBName;
        private SQLiteConnection conn;

        public DataStore()
        {
             if ( !File.Exists(DBName))
            {
                System.Data.SQLite.SQLiteConnection.CreateFile(DBName);
                var conn = new System.Data.SQLite.SQLiteConnection(DBPath);
                conn.Open();
                InsertData(conn);
                conn.Close();
            }

            conn = new System.Data.SQLite.SQLiteConnection(DBPath);

        }

        private void InsertData(SQLiteConnection conn)
        {
            string sql = "create table tb_config(id INTEGER primary key autoincrement,name varchar(50),val varchar(50))";
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = sql;
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();

            string[] datas = { "set_minute,50"};
            foreach (var data in datas)
            {
                var name = data.Split(',')[0];
                var val = data.Split(',')[1];
                sql = string.Format("insert into tb_config (name,val) values('{0}','{1}')", name, val);
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
            
        }

        public int ReadMinute()
        {
            int val = 0;
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("select val from tb_config where name='set_minute'",conn);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
                val = int.Parse( reader.GetString(0));
            reader.Close();
            conn.Close();
            return val;
        }

        public void WriteMinute(int minute)
        {
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand( conn);
            cmd.CommandText = "update tb_config set val ='" + minute + "' where name='set_minute'";
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
