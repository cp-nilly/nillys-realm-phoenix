using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using common;
using server.account;
using server.@char;
using server.credits;
using server.guild;
using server.picture;

namespace server
{
    abstract class RequestHandler
    {
        public abstract void HandleRequest(HttpListenerContext context);
        protected void Write(HttpListenerContext txt, string val)
        {
            var buff = Encoding.UTF8.GetBytes(val);
            txt.Response.OutputStream.Write(buff, 0, buff.Length);
        }
    }

    static class RequestHandlers
    {
        public static readonly Dictionary<string, RequestHandler> Handlers = new Dictionary<string, RequestHandler>()
        {
            {"/crossdomain.xml", new crossdomain()},
            {"/char/list", new list()},
            {"/char/delete", new delete()},
            {"/char/fame", new @char.fame()},
            {"/account/register", new register()},
            {"/account/verify", new verify()},
            {"/account/forgotPassword", new forgotPassword()},
            {"/account/sendVerifyEmail", new sendVerifyEmail()},
            {"/account/changePassword", new changePassword()},
            {"/account/purchaseCharSlot", new purchaseCharSlot()},
            {"/account/setName", new setName()},
            {"/credits/getoffers", new getoffers()},
            {"/credits/add", new add()},
            {"/fame/list", new fame.list()},
            {"/picture/get", new get()},
            {"/guild/getBoard", new getBoard()},
            {"/guild/setBoard", new setBoard()},
            {"/guild/listMembers", new listMembers()},
            {"/index.html", new client()},
            {"/realm.swf", new swf()},
            {"/", new client()}
        };
    }
}
