using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.Net.Http;


namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class SupportDialog : IDialog<int>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (message.Text == "Â÷¶}")
            {
                context.Done<object>(null);
            }
            else
            {
                var ticketNumber = new Random().Next(0, 20000);

                await context.PostAsync($"Your message '{message.Text}' was registered. Once we resolve it; we will get back to you.");

                context.Done(ticketNumber);
            }

        }
    }
}