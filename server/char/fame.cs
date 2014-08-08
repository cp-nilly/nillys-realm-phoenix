using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using db;
using MySql.Data.MySqlClient;

namespace server.@char
{
    internal class fame : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            NameValueCollection query;
            using (var rdr = new StreamReader(context.Request.InputStream))
                query = HttpUtility.ParseQueryString(rdr.ReadToEnd());
            using (var db = new Database())
            {
                Account acc = db.GetAccount(int.Parse(query["accountId"]));
                Char chr = db.LoadCharacter(acc, int.Parse(query["charId"]));

                MySqlCommand cmd = db.CreateQuery();
                cmd.CommandText = @"SELECT time, killer, firstBorn FROM death WHERE accId=@accId AND chrId=@charId;";
                cmd.Parameters.AddWithValue("@accId", query["accountId"]);
                cmd.Parameters.AddWithValue("@charId", query["charId"]);
                int time;
                string killer;
                bool firstBorn;
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    time = Database.DateTimeToUnixTimestamp(rdr.GetDateTime("time"));
                    killer = rdr.GetString("killer");
                    firstBorn = rdr.GetBoolean("firstBorn");
                }
                db.Dispose();
                using (var wtr = new StreamWriter(context.Response.OutputStream))
                    wtr.Write(chr.FameStats.Serialize(acc, chr, time, killer, firstBorn));
            }
        }
    }
}