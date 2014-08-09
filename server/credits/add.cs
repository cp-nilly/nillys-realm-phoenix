using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using common;
using MySql.Data.MySqlClient;

namespace server.credits
{
    internal class add : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            string status;
            using (var db = new Database())
            {
                NameValueCollection query = HttpUtility.ParseQueryString(context.Request.Url.Query);

                MySqlCommand cmd = db.CreateQuery();
                cmd.CommandText = "SELECT id FROM accounts WHERE uuid=@uuid";
                cmd.Parameters.AddWithValue("@uuid", query["guid"]);
                object id = cmd.ExecuteScalar();

                if (id != null)
                {
                    try
                    {
                        //int amount = int.Parse(query["jwt"]);
                        //cmd = db.CreateQuery();
                        //cmd.CommandText = "UPDATE stats SET credits = credits + @amount WHERE accId=@accId";
                        //cmd.Parameters.AddWithValue("@accId", (int)id);
                        //cmd.Parameters.AddWithValue("@amount", amount);
                        //int result = cmd.ExecuteNonQuery();
                        //if (result > 0)
                        //    status = "Ya done...";
                        //else
                        //    status = "Internal error :(";
                        status = "Yeah... We kind of fixed this...";
                    }
                    catch
                    {
                        status = "Yeah... We kind of fixed this...";
                    }
                }
                else
                {
                    status = "Yeah... We kind of fixed this...";
                }
                db.Dispose();
            }

            byte[] res = Encoding.UTF8.GetBytes(
                @"<html>
    <head>
        <title>Nope</title>
        <script>window.close();</script>
    </head>
    <body style='background: #333333'>
        <h1 style='color: #EEEEEE; text-align: center'>
            " + status + @"
        </h1>
    </body>
</html>");
            context.Response.OutputStream.Write(res, 0, res.Length);
        }
    }
}