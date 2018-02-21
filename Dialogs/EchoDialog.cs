using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.Net.Http;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;
        public async Task StartAsync(IDialogContext context)
        {
            string TestValue;
            await context.PostAsync("歡迎來到一般對話模式");
            //var EchoFormDialog = FormDialog.FromForm("",FormOptions.PromptInStart);
            if (!context.ConversationData.TryGetValue(ContextValue.ContextKeyValue,out TestValue))
            {
                TestValue = "英雄聯盟";
                context.ConversationData.SetValue(ContextValue.ContextKeyValue,TestValue);
            }

            await context.PostAsync($"我們來聊聊關於 {TestValue} 的話題吧!!");
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "測試")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "測試成功",
                    "請選擇 Yes or No",
                    promptStyle: PromptStyle.Auto);
            }
            else if (message.Text == "離開")
            {
                context.Done<object>(null);
            }
            else
            {
                await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}