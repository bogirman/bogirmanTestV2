using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using System.Linq;
using System;
using System.Data;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        JDB Main = new JDB();
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // check if activity is of type message
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Root());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                if (message.MembersAdded.Any(o => o.Id == message.Recipient.Id))
                {
                    var RootDialog_Welcome_Message = "你好，我是問答小助手，很高興為您服務，您可以直接問我問題或輸入 help 來查看我的功能。";
                    var reply = message.CreateReply(RootDialog_Welcome_Message);
                    ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                    connector.Conversations.ReplyToActivityAsync(reply);
                }
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
        //public class SQLConnect : MessagesController
        //{
            string ReString = "";
            int ReInt = 0;
            DataTable DT;
            public string Scalar(string str)
            {
                ReString = Main.Scalar(str);
                return ReString;
            }
            public int NonQuery(string str)
            {
                ReInt = Main.NonQuery(str);
                return ReInt;
            }
            public DataTable GetDataSet(string str)
            {
                DT = Main.GetDataSet(str);
                return DT;
            }
        //}
        
        
    }
}