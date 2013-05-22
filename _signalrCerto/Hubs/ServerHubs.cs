using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;


namespace _signalrCerto.Hubs
{
    public class ServerHubs : Hub
    {
        private static List<UserChat> ListUserChat = new List<UserChat>();

        public void addNewUser(string user)
        {
            // add new user in list of users
            ListUserChat.Add(new UserChat { Codigo = Context.ConnectionId, User = user });
            // return new user login for all users connected
            Clients.AllExcept(Context.ConnectionId).addNewUserClient(user, Context.ConnectionId);
            // return list of user for new user connected
            var users = ListUserChat.Where(o => o.Codigo != Context.ConnectionId).ToList<UserChat>();
            Clients.Client(Context.ConnectionId).allUserConnected(
                new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(users).ToString());
        }

        public void sendMsg(string msg,string forUser, bool bPrivate)
        {
            if (bPrivate && forUser != "Todos")
            {
                Clients.Caller.sendForUser(
                    msg,
                    ListUserChat.Where(o => o.Codigo.Equals(Context.ConnectionId)).FirstOrDefault<UserChat>().User,
                    ListUserChat.Where(o => o.Codigo.Equals(forUser)).FirstOrDefault<UserChat>().User);

                Clients.Client(forUser).sendForUser(
                    msg,
                    ListUserChat.Where(o => o.Codigo.Equals(Context.ConnectionId)).FirstOrDefault<UserChat>().User,
                    ListUserChat.Where(o => o.Codigo.Equals(forUser)).FirstOrDefault<UserChat>().User);
            }
            else
            {
                Clients.All.sendMsgForAll(
                    msg,
                    ListUserChat.Where(o => o.Codigo.Equals(Context.ConnectionId)).FirstOrDefault<UserChat>().User,
                    (forUser == "Todos") ? "Todos" : ListUserChat.Where(o => o.Codigo.Equals(forUser)).FirstOrDefault<UserChat>().User);
            }
        }

        public void logout()
        {
            var obj = ListUserChat.Where(o => o.Codigo.Equals(Context.ConnectionId)).FirstOrDefault<UserChat>();
            if(obj != null)
                ListUserChat.Remove(obj);
            Clients.AllExcept(Context.ConnectionId).removeUser(Context.ConnectionId);
        }
    }

    public class UserChat 
    {
        public string Codigo { get; set; }
        public string User { get; set; }
    }
}