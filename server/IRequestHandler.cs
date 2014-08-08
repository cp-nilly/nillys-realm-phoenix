using System.Collections.Generic;
using System.Net;
using server.account;
using server.@char;
using server.credits;
using server.guild;
using server.picture;

namespace server
{
    internal interface IRequestHandler
    {
        void HandleRequest(HttpListenerContext context);
    }

    internal static class RequestHandlers
    {
        public static readonly Dictionary<string, IRequestHandler> Handlers = new Dictionary<string, IRequestHandler>
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
            {"/guild/listMembers", new listMembers()}
        };
    }
}