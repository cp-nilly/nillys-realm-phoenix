using System.Net;
using System.Text;

namespace server
{
    internal class crossdomain : RequestHandler
    {
        public override void HandleRequest(HttpListenerContext context)
        {
            byte[] status = Encoding.UTF8.GetBytes(@"<cross-domain-policy>
	<site-control permitted-cross-domain-policies=""master-only""/>
	<allow-access-from domain=""*"" secure=""false""/>
	<allow-http-request-headers-from domain=""*"" headers=""*"" secure=""false""/>
</cross-domain-policy>");
            context.Response.ContentType = "text/*";
            context.Response.OutputStream.Write(status, 0, status.Length);
        }
    }
}