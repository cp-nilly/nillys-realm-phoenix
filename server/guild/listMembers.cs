using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using db;

namespace server.guild
{
    internal class listMembers : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            NameValueCollection query;
            using (var rdr = new StreamReader(context.Request.InputStream))
                query = HttpUtility.ParseQueryString(rdr.ReadToEnd());

            using (var db = new Database())
            {
                Account acc = db.Verify(query["guid"], query["password"]);
                byte[] status;
                if (acc == null)
                    status = Encoding.UTF8.GetBytes("<Error>Bad login</Error>");
                else
                {
                    try
                    {
                        status =
                            Encoding.UTF8.GetBytes(db.HttpGetGuildMembers(Convert.ToInt32(query["num"]),
                                Convert.ToInt32(query["offset"]), acc));
                    }
                    catch
                    {
                        status = Encoding.UTF8.GetBytes("<Error>Guild member error</Error>");
                    }
                }
                context.Response.OutputStream.Write(status, 0, status.Length);
                context.Response.Close();
            }
        }
    }
}