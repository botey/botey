#r "Newtonsoft.Json"
#load "BasicLuisDialog.csx"

using System;
using System.Data;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions;
using System.Web.Http;
using System.Net.Http.Formatting;
using static Microsoft.Bot.Connector.Activity;
using Microsoft.Bot.Builder.History;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Web;


public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");
    // Initialize the azure bot
    using (BotService.Initialize())
    {
        // Deserialize the incoming activity
        string jsonContent = await req.Content.ReadAsStringAsync();
        
        var activity = JsonConvert.DeserializeObject<Activity>(jsonContent);
        
        // authenticate incoming request and add activity.ServiceUrl to MicrosoftAppCredentials.TrustedHostNames
        // if request is authenticated
        if (!await BotService.Authenticator.TryAuthenticateAsync(req, new [] {activity}, CancellationToken.None))
        {
            return BotAuthenticator.GenerateUnauthorizedResponse(req);
        }
    
        if (activity != null)
        {
            // one of these will have an interface and process it
            switch (activity.GetActivityType())
            {
                case ActivityTypes.Message:
                    //Bot Typing message till the bot is typing 
                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    Activity isTypingReply = activity.CreateReply();
                    isTypingReply.Type = ActivityTypes.Typing;
                    await connector.Conversations.ReplyToActivityAsync(isTypingReply);
                    
                    //var bot = new builder.UniversalBot(connector);
                    /*var newRecipient = update.Recipient.ID?.Where(t => t.Id != activity.Recipient.Id);
                    Activity RecipientId = activity.CreateReply();
                    Recipient.Id.recipientId = ActivityTypes.Message;
                    await connector.Conversations.ReplyToActivityAsync(channelId);
                    //var conversationId = await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount);
                    */
                    //Call BasicLuisDialog.csx
                    StateClient stateClient = activity.GetStateClient();
                    BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                    //userData.SetProperty<bool>("SentGreeting", true);
                    //await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                    //await connector.Conversations.ReplyToActivityAsync(userData);
                            
                    
                   
                    
                    await Conversation.SendAsync(activity, () => new BasicLuisDialog());
                    
                    break;
                case ActivityTypes.ConversationUpdate:
                    var client = new ConnectorClient(new Uri(activity.ServiceUrl));
                    IConversationUpdateActivity update = activity;
                    
                    if (update.MembersAdded.Any())
                    {
                        var reply = activity.CreateReply();
                        var newMembers = update.MembersAdded?.Where(t => t.Id != activity.Recipient.Id);
                        foreach (var newMember in newMembers)
                        {
                            reply.Text = "Welcome";
                            if (!string.IsNullOrEmpty(newMember.Name))
                            {
                                reply.Text += $" {newMember.Name}";
                            }
                            reply.Text += "!";
                            await client.Conversations.ReplyToActivityAsync(reply);
                       
                            
                        }
                        
                    }
                    break;
                case ActivityTypes.ContactRelationUpdate:
                case ActivityTypes.Typing:
                 //sendTyping: true;
                //case ActivityTypes.DeleteUserData:
                case ActivityTypes.Ping:
                default:
                    log.Error($"Unknown activity type ignored: {activity.GetActivityType()}");
                    break;
            }
        }
        return req.CreateResponse(HttpStatusCode.Accepted);
    }  
    
}
  
/*private Activity HandleSystemMessage(Activity message)
{
    if (message.Type == ActivityTypes.DeleteUserData)
    {
        // Implement user deletion here
        // If we handle user deletion, return a real message
    }
    else if (message.Type == ActivityTypes.ConversationUpdate)
    {
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
}*/
