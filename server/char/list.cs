using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using common;

namespace server.@char
{
    internal class list : RequestHandler
    {
        public override void HandleRequest(HttpListenerContext context)
        {
            NameValueCollection query;
            using (var rdr = new StreamReader(context.Request.InputStream))
                query = HttpUtility.ParseQueryString(rdr.ReadToEnd());

            using (var db = new Database())
            {
                List<ServerItem> filteredServers = null;
                Account a = db.Verify(query["guid"], query["password"]);
                if (a != null)
                {
                    if (a.Banned)
                        filteredServers = YoureBanned();
                    else
                        filteredServers = GetServersForRank(a.Rank);
                }
                else
                {
                    filteredServers = GetServersForRank(0);
                }

                var chrs = new Chars
                {
                    Characters = new List<Char>(),
                    NextCharId = 2,
                    MaxNumChars = 1,
                    Account = db.Verify(query["guid"], query["password"]),
                    Servers = filteredServers
                };
                Account dvh = null;
                if (chrs.Account != null)
                {
                    db.GetCharData(chrs.Account, chrs);
                    db.LoadCharacters(chrs.Account, chrs);
                    chrs.News = db.GetNews(chrs.Account);
                    dvh = chrs.Account;
                }
                else
                {
                    chrs.Account = db.CreateGuestAccount(query["guid"]);
                    chrs.News = db.GetNews(null);
                }

                var ms = new MemoryStream();
                var serializer = new XmlSerializer(chrs.GetType(),
                    new XmlRootAttribute(chrs.GetType().Name) {Namespace = ""});

                var xws = new XmlWriterSettings();
                xws.OmitXmlDeclaration = true;
                xws.Encoding = Encoding.UTF8;
                XmlWriter xtw = XmlWriter.Create(context.Response.OutputStream, xws);
                serializer.Serialize(xtw, chrs, chrs.Namespaces);
                db.Dispose();
            }
        }

        static List<ServerItem> GetServersForRank(int r)
        {
            List<ServerItem> slist = GetServerList();
            var removedServers = slist.Where(i => i.RankRequired > r).ToList();

            foreach (ServerItem i in removedServers)
                slist.Remove(i);

            return slist;
        }

        static List<ServerItem> GetServerList()
        {
            var ret = new List<ServerItem>();
            int num = Program.Settings.GetValue<int>("svrNum");
            for (int i = 0; i < num; i++)
                ret.Add(new ServerItem()
                {
                    Name = Program.Settings.GetValue("svr" + i + "Name"),
                    Lat = Program.Settings.GetValue<int>("svr" + i + "Lat", "0"),
                    Long = Program.Settings.GetValue<int>("svr" + i + "Long", "0"),
                    DNS = Program.Settings.GetValue("svr" + i + "Adr", "127.0.0.1"),
                    Usage = 0.2,
                    AdminOnly = Program.Settings.GetValue<bool>("svr" + i + "Admin", "false"),
                    RankRequired = Program.Settings.GetValue<int>("svr" + i + "Rank", "0")
                });
            return ret;
        }

        static List<ServerItem> YoureBanned()
        {
            var servers
                = new List<ServerItem>
                {
                    new ServerItem
                    {
                        Name = "You're Banned!",
                        Lat = 22.28,
                        Long = 114.16,
                        DNS = "",
                        Usage = 0.2,
                        AdminOnly = false
                    }
                };
            return servers;
        }
    }
}
