using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Text;
using System.IO;
using System.Web.UI;

namespace server
{
    internal class client : RequestHandler
    {
        public override void HandleRequest(HttpListenerContext context)
        {
            byte[] res = File.ReadAllBytes("data/index.html");
            context.Response.OutputStream.Write(res, 0, res.Length);
        }
    }

    internal class swf : RequestHandler
    {
        public override void HandleRequest(HttpListenerContext context)
        {
            byte[] res = File.ReadAllBytes("data/realm.swf");
            context.Response.OutputStream.Write(res, 0, res.Length);
        }
    }
}
