using System.Net;
using System.Text;

namespace server
{
    internal class crossdomain : RequestHandler
    {
        public override void HandleRequest(HttpListenerContext context)
        {
            byte[] status = Encoding.UTF8.GetBytes(@"<cross-domain-policy><allow-access-from domain=""*""/></cross-domain-policy>");
            context.Response.ContentType = "text/*";
            context.Response.OutputStream.Write(status, 0, status.Length);
        }
    }
}