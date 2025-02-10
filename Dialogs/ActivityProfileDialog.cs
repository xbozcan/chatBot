using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using EchoBot.Data;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace EchoBot.Dialogs;

public class ActivityProfileDialog : ComponentDialog
{
    public ActivityProfileDialog() : base(nameof(ActivityProfileDialog))
    {
        var waterfallSteps = new WaterfallStep[]
        {
            AskActivityTypeStepAsync,
            AskScopeStepAsync,
            AskProjectStepAsync,
            AskModuleStepAsync,
            AskLocationStepAsync,
            AskActivityHourStepAsync,
            AskDescriptionStepAsync,
            SummarizeActivityProfileStepAsync
        };
        
        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
        AddDialog(new TextPrompt(nameof(TextPrompt)));
    }
    
    public bool IsDialogCompleted = false;

   /* private async Task<DialogTurnResult> SendWelcomeMessageStepAsync(WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Aktivite girme ekranına hoşgeldiniz."), cancellationToken);
        return await stepContext.NextAsync(null,cancellationToken);
    } */
    private async Task<DialogTurnResult> AskActivityTypeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var options = new List<string> { "Danışmanlık", "Satış Aktivitesi", "Eğitim" };

        return await stepContext.PromptAsync(nameof(ChoicePrompt),

            new PromptOptions
            {
                Prompt = MessageFactory.Text("Aktivite tipini seçiniz"),
                Choices = ChoiceFactory.ToChoices(options),
                Style = ListStyle.HeroCard,
            },
            cancellationToken
        );
    }

    private async Task<DialogTurnResult> AskScopeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        stepContext.Values["ActivityType"] = ( (FoundChoice)stepContext.Result).Value;

        return await stepContext.PromptAsync(nameof(ChoicePrompt),


            new PromptOptions
            {
                Prompt = MessageFactory.Text("Kapsam seçiniz"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Uyarlama", "Geliştirme" }),
                Style = ListStyle.HeroCard,
            }, cancellationToken
        );
    }

    private async Task<DialogTurnResult> AskProjectStepAsync(WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        
        stepContext.Values["Scope"] = ( (FoundChoice)stepContext.Result).Value;

        return await stepContext.PromptAsync(nameof(ChoicePrompt),


            new PromptOptions
            {
                Prompt = MessageFactory.Text("Proje seçiniz"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Project A", "Project B" }),
                Style = ListStyle.HeroCard,
            }, cancellationToken
        );
    }

    private async Task<DialogTurnResult> AskModuleStepAsync(WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        stepContext.Values["Project"] = ( (FoundChoice)stepContext.Result).Value;

        return await stepContext.PromptAsync(nameof(ChoicePrompt),


            new PromptOptions
            {
                Prompt = MessageFactory.Text("Modül seçiniz"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "ABAP", "RPA" }),
                Style = ListStyle.HeroCard,
            }, cancellationToken
        );
    }

    private async Task<DialogTurnResult> AskLocationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        stepContext.Values["Module"] = ( (FoundChoice)stepContext.Result).Value;

        return await stepContext.PromptAsync(nameof(ChoicePrompt),


            new PromptOptions
            {
                Prompt = MessageFactory.Text("Lokasyon seçiniz"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Ev", "Ofis" }),
                Style = ListStyle.HeroCard,
            }, cancellationToken
        );
    }

    private async Task<DialogTurnResult> AskActivityHourStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        stepContext.Values["Location"] = ( (FoundChoice)stepContext.Result).Value;

        return await stepContext.PromptAsync(nameof(ChoicePrompt),


            new PromptOptions
            {
                Prompt = MessageFactory.Text("Aktivite saati seçiniz"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1", "2","3","4","5","6","7","8" }),
                Style = ListStyle.HeroCard,
            }, cancellationToken
        );
    }

    private async Task<DialogTurnResult> AskDescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        stepContext.Values["ActivityHour"] = ( (FoundChoice)stepContext.Result).Value;

        return await stepContext.PromptAsync(nameof(TextPrompt),
            new PromptOptions
            {
                Prompt = MessageFactory.Text("Açıklama giriniz"),
                
            }, cancellationToken
        );
    }

    private async Task<DialogTurnResult> SummarizeActivityProfileStepAsync(WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    { 
         stepContext.Values["Description"] = (string)stepContext.Result;

        var activityProfile = new ActivityProfile // null dönüyor 
        {
            ActivityType = (string)stepContext.Values["ActivityType"],
            Scope = (string)stepContext.Values["Scope"],
            Project = (string)stepContext.Values["Project"],
            Module = (string)stepContext.Values["Module"],
            Location = (string)stepContext.Values["Location"],
            ActivityHour = (string)stepContext.Values["ActivityHour"],
            Description = (string)stepContext.Values["Description"],
        };
        
        var adaptiveCard = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
        {
            Body = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock
                {
                    Text = $"Activity Type: {activityProfile.ActivityType}",
                    Size = AdaptiveTextSize.Medium,
                    Wrap = true
                },
                new AdaptiveTextBlock
                {
                    Text = $"Scope: {activityProfile.Scope}",
                    Size = AdaptiveTextSize.Medium,
                    Wrap = true
                },
                new AdaptiveTextBlock
                {
                    Text = $"Project: {activityProfile.Project}",
                    Size = AdaptiveTextSize.Medium,
                    Wrap = true
                },
                new AdaptiveTextBlock
                {
                    Text = $"Module: {activityProfile.Module}",
                    Size = AdaptiveTextSize.Medium,
                    Wrap = true
                },
                new AdaptiveTextBlock
                {
                    Text = $"Location: {activityProfile.Location}",
                    Size = AdaptiveTextSize.Medium,
                    Wrap = true
                },
                new AdaptiveTextBlock
                {
                    Text = $"Activity Hour: {activityProfile.ActivityHour}",
                    Size = AdaptiveTextSize.Medium,
                    Wrap = true
                },
                new AdaptiveTextBlock
                {
                    Text = $"Description: {activityProfile.Description}",
                    Size = AdaptiveTextSize.Medium,
                    Wrap = true
                }
            }
        };

        var message = MessageFactory.Attachment(new Attachment
        {
            ContentType = "application/vnd.microsoft.card.adaptive",
            Content = adaptiveCard
        });

        await stepContext.Context.SendActivityAsync(message, cancellationToken);
        
        IsDialogCompleted = true;
        return await stepContext.EndDialogAsync(null,cancellationToken);

    }
}