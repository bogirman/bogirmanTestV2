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
            await context.PostAsync("�w��Ө�@���ܼҦ�");
            //var EchoFormDialog = FormDialog.FromForm("",FormOptions.PromptInStart);
            if (!context.ConversationData.TryGetValue(ContextValue.ContextKeyValue,out TestValue))
            {
                TestValue = "�^���p��";
                context.ConversationData.SetValue(ContextValue.ContextKeyValue,TestValue);
            }

            await context.PostAsync($"�ڭ̨Ӳ������ {TestValue} �����D�a!!");
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "����")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "���զ��\",
                    "�п�� Yes or No",
                    promptStyle: PromptStyle.Auto);
            }
            else if (message.Text == "���}")
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