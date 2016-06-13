using Codeplex.Data;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetFriendInfo.Models
{
    class SqliteAccesser
    {
        private static string dbFileName = Properties.Settings.Default.DatabaseFile;
        private static string connStr = @"Data Source=" + dbFileName;

        public static void InitDatabase()
        {
            // DBファイルが存在する場合削除する
            var dbfile = new FileInfo(dbFileName);
            if (dbfile.Exists)
            {
                dbfile.Delete();
            }

            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                using (SQLiteCommand command = conn.CreateCommand())
                {
                    command.CommandText = "create table members(id INTEGER  PRIMARY KEY AUTOINCREMENT, number TEXT, board TEXT, name TEXT, email TEXT)";
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static IEnumerable<Member> SelectMembers()
        {
            var members = DbExecutor.ExecuteReader(new SQLiteConnection(connStr), @"select * from members")
                .Select(dr =>
                {
                    var member = new Member();
                    member.Id = (long)dr["id"];
                    member.Number = dr["number"] as string;
                    member.Board = dr["board"] as string;
                    member.Name = dr["name"] as string;
                    member.Email = dr["email"] as string;
                    return member;
                });

            return members;
        }

        public static void InsertMember(Member member)
        {
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                using (SQLiteTransaction tran = conn.BeginTransaction())
                {
                    using (SQLiteCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "insert into members (number, board, name) values (@number, @board, @name)";
                        command.Parameters.Add("number", System.Data.DbType.String);
                        command.Parameters.Add("board", System.Data.DbType.String);
                        command.Parameters.Add("name", System.Data.DbType.String);
                        command.Parameters["number"].Value = member.Number;
                        command.Parameters["board"].Value = member.Board;
                        command.Parameters["name"].Value = member.Name;
                        command.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                conn.Close();
            }
        }

        public static void UpdateMember(Member member)
        {
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                using (SQLiteTransaction tran = conn.BeginTransaction())
                {
                    using (SQLiteCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "update members set board = @board, name = @name where number = @number";
                        command.Parameters.Add("number", System.Data.DbType.String);
                        command.Parameters.Add("board", System.Data.DbType.String);
                        command.Parameters.Add("name", System.Data.DbType.String);
                        command.Parameters["number"].Value = member.Number;
                        command.Parameters["board"].Value = member.Board;
                        command.Parameters["name"].Value = member.Name;
                        command.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                conn.Close();
            }
        }

        public static void DeleteMember(Member member)
        {
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                using (SQLiteTransaction tran = conn.BeginTransaction())
                {
                    using (SQLiteCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "delete from members where number = @number";
                        command.Parameters.Add("number", System.Data.DbType.String);
                        command.Parameters["number"].Value = member.Number;
                        command.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                conn.Close();
            }
        }
    }
}
