using CommonMessage;
using MessageBus.Interfaces;
using MessageBus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessageBus.APIs
{
    public static class MessageApi
    {
        public static void RegisterMessageApi(this WebApplication app)
        {
            app.MapGet("/messages", GetAllMessages)
                .Produces<List<Message>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapGet("/messages/{id}", GetMessage)
                .Produces<Message>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapPost("/publish", Publish)
                .Produces<Message>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }

        public async static Task<IResult> GetAllMessages(HttpRequest request, [FromServices] DataContext db)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request GetAllMessages");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                List<Message> messages = await db.Messages
                        .Include(m => m.Channel)
                        .Include(m => m.Subscribers)
                        .ThenInclude(ms => ms.Subscriber)
                        .ToListAsync();

                response = new JsonResponseBuilder(messages).Build<List<Message>>();
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when retrieving all messages.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response GetAllMessages [{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> GetMessage(HttpRequest request, [FromServices] DataContext db, int id)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request GetMessage {{ id: {id} }}");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                Message messageObj;
                try
                {
                    messageObj = await db.Messages
                        .Where(m => m.Id == id)
                        .Include(m => m.Channel)
                        .Include(m => m.Subscribers)
                        .ThenInclude(ms => ms.Subscriber)
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No message with id " + id);
                }

                response = new JsonResponseBuilder(messageObj).Build<Message>();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when retrieving message.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when retrieving message.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response GetMessage {{ id: {id} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> Publish(HttpRequest request, [FromServices]DataContext db, 
            [FromServices]IBackgroundMessageQueue queue, PublishDto data)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request Publish {{ PublishDto: {JsonFormatter.ToString(data)} }}");

            var statusCode = StatusCodes.Status201Created;
            var message = "success";
            object response = null;
            try
            {
                Channel channel;
                try
                {
                    channel = await db.Channels
                        .Where(c => c.DeletedAt == null && c.Id == data.ChannelId)
                        .Include(c => c.Subscribers.Where(s => s.DeletedAt == null))
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No channel with id " + data.ChannelId);
                }

                var messageObj = new Message();
                messageObj.PublisherName = data.PublisherName;
                messageObj.Channel = channel;
                messageObj.Data = data.Data;

                await db.Messages.AddAsync(messageObj);

                var messageSubscribers = new List<MessageSubscriber>();
                foreach (var subscriber in channel.Subscribers)
                {
                    var messageSubscriber = new MessageSubscriber();
                    messageSubscriber.Subscriber = subscriber;
                    messageSubscriber.Message = messageObj;
                    await db.MessageSubscribers.AddAsync(messageSubscriber);
                    messageSubscribers.Add(messageSubscriber);
                }

                await db.SaveChangesAsync();

                message = $"/messages/{messageObj.Id}";
                response = new JsonResponseBuilder(messageObj).Build<Message>();

                foreach (var messageSubscriber in messageSubscribers)
                {
                    queue.EnqueueAsync(messageSubscriber);
                }
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when publishing message.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when publishing message.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response Publish {{ PublishDto: {JsonFormatter.ToString(data)} }}" +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }
    }
}
