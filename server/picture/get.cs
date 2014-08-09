using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using common.data;

namespace server.picture
{
    internal class get : RequestHandler
    {
        private readonly byte[] buff = new byte[0x10000];

        public override void HandleRequest(HttpListenerContext context)
        {
            NameValueCollection query;
            using (var rdr = new StreamReader(context.Request.InputStream))
                query = HttpUtility.ParseQueryString(rdr.ReadToEnd());

            //warning: maybe has hidden url injection
            string id = query["id"];

            if (!id.StartsWith("draw:") && !id.StartsWith("file:") && !id.StartsWith("tdraw:"))
            {
                string path = Path.GetFullPath("texture/_" + id + ".png");
                if (!File.Exists(path))
                {
                    byte[] status = Encoding.UTF8.GetBytes("<Error>Invalid ID.</Error>");
                    context.Response.OutputStream.Write(status, 0, status.Length);
                    return;
                }
                using (FileStream i = File.OpenRead(path))
                {
                    int c;
                    while ((c = i.Read(buff, 0, buff.Length)) > 0)
                        context.Response.OutputStream.Write(buff, 0, c);
                }
            }
            else
            {
                string[] rid = id.Split(':');
                if (XmlDatas.RemoteTextures.ContainsKey(id))
                    context.Response.OutputStream.Write(XmlDatas.RemoteTextures[id], 0,
                        XmlDatas.RemoteTextures[id].Length);
                else if (rid.Length > 1)
                    if (rid[0] == "draw")
                        XmlDatas.RemoteTextures.Add(id,
                            new WebClient().DownloadData("http://realmofthemadgod.appspot.com/picture/get?id=" + rid[1]));
                    else if (rid[0] == "tdraw")
                        XmlDatas.RemoteTextures.Add(id,
                            new WebClient().DownloadData("http://rotmgtesting.appspot.com/picture/get?id=" + rid[1]));
                    else
                        XmlDatas.RemoteTextures.Add(id, File.ReadAllBytes("texture/" + rid[1] + ".png"));
            }
        }
    }
}