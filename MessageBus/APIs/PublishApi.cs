using CommonMessage.FormRequest;
using MessageBus.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageBus.APIs
{
    public static class PublishApi
    {
        public static void RegisterPublishApi(this WebApplication app)
        {
            app.MapPost("/publish", async (DataContext db, PostPublish request) =>
            {
                var channel = await db.Channels
                    .FindAsync(request.ChannelId);
                if (channel == null)
                {
                    return Results.BadRequest("channel is not found");
                }

                var message = new Message();
                message.PublisherName = request.Name;
                message.Channel = channel;

                db.Messages.Add(message);

                var subscribers = await db.Subscribers.Where(s => s.Channels == channel).ToListAsync();

                foreach (var subscriber in subscribers)
                {
                    var messageSubscriber = new MessageSubscriber();
                    messageSubscriber.Subscriber = subscriber;
                    messageSubscriber.Message = message;
                    db.MessageSubscribers.Add(messageSubscriber);
                }
                await db.SaveChangesAsync();

                return Results.Ok();
            });
        }
    }
}
