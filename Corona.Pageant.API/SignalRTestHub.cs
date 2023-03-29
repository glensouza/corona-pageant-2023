using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Net;

namespace Corona.Pageant.API;

public class SignalRTestHub : ServerlessHub
{
    [FunctionName("negotiate")]
    [ProducesResponseType(typeof(SignalRConnectionInfo), (int)HttpStatusCode.OK)]
    public SignalRConnectionInfo Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
        [SignalRConnectionInfo(HubName = "pageant")] SignalRConnectionInfo connectionInfo)
    {
        return connectionInfo;
    }

    //[FunctionName(nameof(OnConnected))]
    //public async Task OnConnected([SignalRTrigger] InvocationContext invocationContext, ILogger logger)
    //{
    //    await Clients.All.SendAsync(NewConnectionTarget, new NewConnection(invocationContext.ConnectionId));
    //    logger.LogInformation($"{invocationContext.ConnectionId} has connected");
    //}

    //[FunctionName(nameof(Broadcast))]
    //public async Task Broadcast([SignalRTrigger] InvocationContext invocationContext, string message, ILogger logger)
    //{
    //    await Clients.All.SendAsync(NewMessageTarget, new NewMessage(invocationContext, message));
    //    logger.LogInformation($"{invocationContext.ConnectionId} broadcast {message}");
    //}

    //[FunctionName(nameof(OnDisconnected))]
    //public void OnDisconnected([SignalRTrigger] InvocationContext invocationContext)
    //{
    //}


    //[FunctionName("reset")]
    //[QueryStringParameter("reset", "Reset", DataType = typeof(string), Required = true)]
    //public static void Reset([HttpTrigger(AuthorizationLevel.Function, "post")] object message,
    //    [SignalR(HubName = "pageant")] IAsyncCollector<SignalRMessage> signalRMessages)
    //{
    //    signalRMessages.AddAsync(
    //        new SignalRMessage
    //        {
    //            Target = "reset",
    //            Arguments = new[] { message }
    //        });
    //}

    //[FunctionName("ready")]
    //[QueryStringParameter("ready", "Ready", DataType = typeof(string), Required = true)]
    //[ProducesDefaultResponseType]
    //public static void Ready([HttpTrigger(AuthorizationLevel.Function, "post")] SignalRCommunication message,
    //    [SignalR(HubName = "pageant")] IAsyncCollector<SignalRMessage> signalRMessages)
    //{
    //    signalRMessages.AddAsync(
    //        new SignalRMessage
    //        {
    //            Target = "ready",
    //            UserId = message.SessionId.ToString(),
    //            Arguments = new[] { message.Record }
    //        });
    //}

    //// TODO: Swagger
    //[FunctionName("transcriptions")]
    //public static void SendTranscription(
    //    [HttpTrigger(AuthorizationLevel.Function, "post")] SignalRCommunication transcription,
    //    [SignalR(HubName = "translator")] IAsyncCollector<SignalRMessage> signalRMessages)
    //{
    //    signalRMessages.AddAsync(
    //        new SignalRMessage
    //        {
    //            Target = "newTranscription",
    //            UserId = transcription.SessionId.ToString(),
    //            Arguments = new[] { transcription.Record }
    //        });
    //}

    //// TODO: Swagger
    //[FunctionName("translations")]
    //public static void SendTranslation(
    //    [HttpTrigger(AuthorizationLevel.Function, "post")] SignalRCommunication translation,
    //    [SignalR(HubName = "translator")] IAsyncCollector<SignalRMessage> signalRMessages)
    //{
    //    signalRMessages.AddAsync(
    //        new SignalRMessage
    //        {
    //            Target = "newTranslation",
    //            UserId = translation.SessionId.ToString(),
    //            Arguments = new[] { translation.Record }
    //        });
    //}
}
