// Generated with EchoBot .NET Template version v4.22.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EchoBot.Data;
using EchoBot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace EchoBot.Bots
{
    public class DialogBot : ActivityHandler
    {
        private readonly ActivityProfileDialog _dialog;
        private readonly ConversationState _conversationState;
        private readonly UserState _userState;
        private readonly IStatePropertyAccessor<DialogState> _dialogStateAccessor;
        
        public DialogBot(ConversationState conversationState, UserState userState, ActivityProfileDialog activityProfileDialog)
        {
            _userState = userState;
            _conversationState = conversationState;
            _dialog = activityProfileDialog;
            _dialogStateAccessor = _conversationState.CreateProperty<DialogState>("DialogState");
        }


        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {

            if (turnContext.Activity.MembersAdded != null && turnContext.Activity.MembersAdded.Any())
            {
                var membersAdded = turnContext.Activity.MembersAdded.Where(member => member.Id != turnContext.Activity.From.Id).ToList();

                foreach (var member in membersAdded)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Aktivite girme ekranına hoşgeldiniz"), cancellationToken);
                }
            }
            await _dialog.RunAsync(turnContext, _dialogStateAccessor, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (_dialog.IsDialogCompleted)
            {
                await turnContext.SendActivityAsync("Aktivite profiliniz tamamlandı. Yeni bir işlem yapmak için tekrar başlatın.", cancellationToken: cancellationToken);
                _dialog.IsDialogCompleted = false;
                return;
            }
            await _dialog.RunAsync(turnContext, _dialogStateAccessor, cancellationToken);
        }
        
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}
