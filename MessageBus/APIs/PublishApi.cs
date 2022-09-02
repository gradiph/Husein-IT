using CommonMessage.FormRequest;
using Microsoft.EntityFrameworkCore;

namespace MessageBus.APIs
{
    public static class PublishApi
    {
        public static void RegisterPublishApi(this WebApplication app)
        {
            app.MapPost("/publish", PublishMessage);
        }

        public async static Task<IResult> PublishMessage(HttpRequest request, DataContext db, PostPublish data)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request PublishMessage {{ PostPublish: {JsonFormatter.ToString(data)} }}");

            var statusCode = 200;
            var message = "success";
            object response = null;
            try
            {
                Channel channel = await db.Channels
                    .Where(c => c.DeletedAt == null && c.Id == data.ChannelId)
                    .Include(c => c.Subscribers.Where(s => s.DeletedAt == null))
                    .FirstAsync();

                var messageObj = new Message();
                messageObj.PublisherName = data.PublisherName;
                messageObj.Channel = channel;
                messageObj.Data = data.Data;

                await db.Messages.AddAsync(messageObj);

                foreach (var subscriber in channel.Subscribers)
                {
                    var messageSubscriber = new MessageSubscriber();
                    messageSubscriber.Subscriber = subscriber;
                    messageSubscriber.Message = messageObj;
                    await db.MessageSubscribers.AddAsync(messageSubscriber);
                }

                await db.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                statusCode = 422;
                message = "No channel with id " + data.ChannelId;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when publishing message.", e);
                statusCode = 500;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response PublishMessage {{ PostPublish: {JsonFormatter.ToString(data)} }}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }
    }
}
