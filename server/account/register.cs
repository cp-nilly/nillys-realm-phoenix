using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using common;
using MySql.Data.MySqlClient;

namespace server.account
{
    internal class register : IRequestHandler
    {
        public static bool IsUsername(string username)
        {
            string pattern;
            // letters,  length between 3 to 12.
            pattern = @"^[a-zA-Z]{3,12}$";

            Regex regex = new Regex(pattern);
            return regex.IsMatch(username);
        }

        public void HandleRequest(HttpListenerContext context)
        {
            NameValueCollection query;
            using (var rdr = new StreamReader(context.Request.InputStream))
                query = HttpUtility.ParseQueryString(rdr.ReadToEnd());

            using (var db = new Database())
            {
                byte[] status;
                if (!IsUsername(query["newGUID"]))
                    status = Encoding.UTF8.GetBytes("<Error>Invalid Username</Error>");
                else
                {
                    if (db.HasUuid(query["guid"]) &&
                        db.Verify(query["guid"], "") != null)
                    {
                        if (db.HasUuid(query["newGUID"]))
                            status = Encoding.UTF8.GetBytes("<Error>Username is already taken!</Error>");
                        else
                        {
                            MySqlCommand cmd = db.CreateQuery();
                            cmd.CommandText =
                                "UPDATE accounts SET uuid=@newUuid, name=@newUuid, password=SHA1(@password), guest=FALSE, namechosen=TRUE  WHERE uuid=@uuid;";
                            cmd.Parameters.AddWithValue("@uuid", query["guid"]); // old uuid
                            cmd.Parameters.AddWithValue("@newUuid", query["newGUID"]); // new name
                            cmd.Parameters.AddWithValue("@password", query["newPassword"]); // new pass

                            if (cmd.ExecuteNonQuery() > 0)
                                status = Encoding.UTF8.GetBytes("<Success />");
                            else
                                status = Encoding.UTF8.GetBytes("<Error>Internal Error</Error>");
                        }
                    }
                    else
                    {
                        if (db.Register(query["newGUID"], query["newPassword"], false) != null)
                            status = Encoding.UTF8.GetBytes("<Success />");
                        else
                            status = Encoding.UTF8.GetBytes("<Error>Internal Error</Error>");
                    }
                }
                context.Response.OutputStream.Write(status, 0, status.Length);
            }
        }
    }
}