using System.Net;
using System.Text;

namespace server.account
{
    internal class forgotPassword : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            byte[] status =
                Encoding.UTF8.GetBytes("<Error>This function has been disabled, please contact a server admin.</Error>");
            context.Response.OutputStream.Write(status, 0, status.Length);
        }
    }
}