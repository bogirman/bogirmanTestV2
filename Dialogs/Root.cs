using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using simpleSendMessage;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class Root : IDialog<object>
    {
        protected int count = 1;
        private const string EchoDialog = "一般對話";
        private const string QuestionAndAnswer = "QA問答";
        private string CB_ID = System.Configuration.ConfigurationManager.AppSettings.Get("ChatBotID").ToString();
        //JDB Main = new JDB();
        [NonSerialized]
        Timer t;
        
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {

            var message = await argument;
            //判斷連線者閒置時間所需資訊
            ConversationStarter.toId = message.From.Id;
            ConversationStarter.toName = message.From.Name;
            ConversationStarter.fromId = message.Recipient.Id;
            ConversationStarter.fromName = message.Recipient.Name;
            ConversationStarter.serviceUrl = message.ServiceUrl;
            ConversationStarter.channelId = message.ChannelId;
            ConversationStarter.conversationId = message.Conversation.Id;

            DataTable DT = new MessagesController().GetDataSet("select * from CB_Mod where CB_ID='" + CB_ID + "'");
            if (message.Text.ToLower().Contains("help") || message.Text.ToLower().Contains("support") || message.Text.ToLower().Contains("problem"))
            {
                await context.Forward(new SupportDialog(), this.ResumeAfterSupportDialog, message, CancellationToken.None);
            }
            else if (message.Text == "一般對話")
            {
                context.Call(new EchoDialog(), this.ResumeAfterOptionDialog);
            }
            else if (message.Text == "QA問答")
            {
                context.Call(new QuestionAndAnswer(), this.ResumeAfterOptionDialog);
            }
            else
            {
                if (DT.Rows.Count > 0)
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        await context.PostAsync($"CB_Mod_Name：{DT.Rows[i]["Mod_Name"].ToString()}");
                    }
                }
                //await context.PostAsync($"{this.count++}: You said {message.Text}");
                //context.Wait(MessageReceivedAsync);
                //ShowOptions(context);
            }
            t = new Timer(new TimerCallback(timerEvent));
            t.Change(50000, Timeout.Infinite);
            await context.PostAsync($"你閒置很久了喔! 要不要我講個笑話給你聽呢?");
            context.Wait(MessageReceivedAsync);
        }
        public void timerEvent(object target)
        {
            t.Dispose();
            ConversationStarter.Resume(ConversationStarter.conversationId, ConversationStarter.channelId); //We don't need to wait for this, just want to start the interruption here
        }
        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { EchoDialog, QuestionAndAnswer }, "請問你需要的服務是下列何者?", "Not a valid option", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case EchoDialog:
                        context.Call(new EchoDialog(), this.ResumeAfterOptionDialog);
                        break;
                    case QuestionAndAnswer:
                        context.Call(new QuestionAndAnswer(), this.ResumeAfterOptionDialog);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");
                
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<int> result)
        {
            var ticketNumber = await result;

            await context.PostAsync($"Thanks for contacting our support team. Your ticket number is {ticketNumber}.");
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

    }
}