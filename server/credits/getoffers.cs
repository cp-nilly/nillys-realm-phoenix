using System.Net;
using System.Text;

namespace server.credits
{
    internal class getoffers : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            byte[] res = Encoding.UTF8.GetBytes(
                "<Offers><Tok>WUT</Tok><Exp>STH</Exp><Offer><Id>0</Id><Price>0</Price><RealmGold>No Gold</RealmGold><CheckoutJWT>No Gold</CheckoutJWT><Data>YO</Data><Currency>HKD</Currency></Offer></Offers>");
            context.Response.OutputStream.Write(res, 0, res.Length);
        }
    }
}