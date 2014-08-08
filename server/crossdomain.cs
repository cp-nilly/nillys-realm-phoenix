using System.Net;
using System.Text;

namespace server
{
    internal class crossdomain : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            byte[] status = Encoding.UTF8.GetBytes(@"<cross-domain-policy><allow-access-from domain=""*""/></cross-domain-policy>");
			//byte[] status = Encoding.UTF8.GetBytes(@"<cross-domain-policy></cross-domain-policy>");
            context.Response.ContentType = "text/*";
            context.Response.OutputStream.Write(status, 0, status.Length);
        }
    }
}