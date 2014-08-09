using System.Net;
using System.Text;

namespace server.account
{
    internal class sendVerifyEmail : RequestHandler
    {
        public override void HandleRequest(HttpListenerContext context)
        {
            byte[] status = Encoding.UTF8.GetBytes("<Error>Nope.</Error>");
            context.Response.OutputStream.Write(status, 0, status.Length);
        }
    }
}