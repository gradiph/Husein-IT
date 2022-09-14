using MessageBus.Models;
using MessageBus.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessageBus.APIs
{
    public static class ChannelApi
    {
        public static void RegisterChannelApi(this WebApplication app)
        {
            app.MapGet("/channels", GetAllChannels)
                .Produces<ICollection<Channel>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapGet("/channels/{id}", GetChannel)
                .Produces<Channel>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapPost("/channels", CreateChannel)
                .Produces<Channel>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapPut("/channels/{id}", UpdateChannel)
                .Produces<Channel>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapDelete("/channels/{id}", SoftDeleteChannel)
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapPost("/channels/{id}/restore", RestoreChannel)
                .Produces<Channel>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapDelete("/channels/{id}/force", DestroyChannel)
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapPost("/channels/{id}/subscribers", AddSubscribers)
                .Produces<Channel>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapDelete("/channels/{id}/subscribers", RemoveSubscribers)
                .Produces<Channel>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }

        public async static Task<IResult> GetAllChannels(HttpRequest request, [FromServices]DataContext db)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream, "Request GetAllChannels");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                List<Channel> channels = await db.Channels
                    .Where(c => c.DeletedAt == null)
                    .ToListAsync();

                response = new JsonResponseBuilder(channels).Build<List<Channel>>();
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when retrieving all channels.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream, 
                $"Response GetAllChannels [{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> GetChannel(HttpRequest request, DataContext db, int id)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream, 
                $"Request GetChannel {{ id: {id} }}");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                Channel channel;
                try
                {
                    channel = await db.Channels
                        .Where(c => c.DeletedAt == null && c.Id == id)
                        .Include(c => c.Subscribers.Where(s => s.DeletedAt == null))
                        .Include(c => c.Messages)
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No channel with id " + id);
                }

                response = new JsonResponseBuilder(channel).Build<Channel>();
            } catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when retrieving channel.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            } catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when retrieving channel.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream, 
                $"Response GetChannel {{ id: {id} }} [{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> CreateChannel(HttpRequest request, DataContext db, ChannelDto channelDto)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request CreateChannel {{ ChannelDto: {JsonFormatter.ToString(channelDto)} }}");

            var statusCode = StatusCodes.Status201Created;
            var message = "success";
            object response = null;
            try
            {
                var channel = channelDto.ToChannel();

                db.Channels.Add(channel);
                await db.SaveChangesAsync();

                message = $"/channels/{channel.Id}";

                response = new JsonResponseBuilder(channel).Build<Channel>();
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when creating channel.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response CreateChannel {{ ChannelDto: {JsonFormatter.ToString(channelDto)} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}"
            );
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> UpdateChannel(HttpRequest request, DataContext db, int id, ChannelDto channelDto)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request UpdateChannel {{ id: {id}, ChannelDto: {JsonFormatter.ToString(channelDto)} }}");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                Channel channel;
                try
                {
                    channel = await db.Channels
                        .Where(c => c.DeletedAt != null && c.Id == id)
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No channel with id " + id);
                }

                channel.Name = channelDto.Name;

                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(channel).Build<Channel>();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when updating channel.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when updating channel.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response UpdateChannel {{ id: {id}, ChannelDto: {JsonFormatter.ToString(channelDto)} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> SoftDeleteChannel(HttpRequest request, DataContext db, int id)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request SoftDeleteChannel {{ id: {id} }}");

            var statusCode = StatusCodes.Status204NoContent;
            var message = "success";
            object response = null;
            try
            {
                Channel channel;
                try
                {
                    channel = await db.Channels
                        .Where(c => c.DeletedAt != null && c.Id == id)
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No channel with id " + id);
                }

                channel.DeletedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when soft deleting channel.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when soft deleting channel.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response SoftDeleteChannel {{ id: {id} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> RestoreChannel(HttpRequest request, DataContext db, int id)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request RestoreChannel {{ id: {id} }}");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                Channel channel;
                try
                {
                    channel = await db.Channels
                        .Where(c => c.DeletedAt != null && c.Id == id)
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No channel with id " + id);
                }

                channel.DeletedAt = null;
                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(channel).Build<Channel>();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when restoring channel.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when restoring channel.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response RestoreChannel {{ id: {id} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> DestroyChannel(HttpRequest request, DataContext db, int id)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request DestroyChannel {{ id: {id} }}");

            var statusCode = StatusCodes.Status204NoContent;
            var message = "success";
            object response = null;
            try
            {
                Channel channel;
                try
                {
                    channel = await db.Channels
                        .Where(c => c.DeletedAt != null && c.Id == id)
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No channel with id " + id);
                }

                db.Channels.Remove(channel);
                await db.SaveChangesAsync();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when destroying channel.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when destroying channel.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response DestroyChannel {{ id: {id} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> AddSubscribers(HttpRequest request, DataContext db, int id, SubscribeChannelDto data)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request AddSubscribers {{ id: {id}, SubscribeChannelDto: {JsonFormatter.ToString(data)} }}");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                Channel channel;
                ICollection<Subscriber> subscribers;

                try
                {
                    channel = await db.Channels
                        .Where(c => c.DeletedAt == null && c.Id == id)
                        .Include(c => c.Subscribers.Where(s => s.DeletedAt == null))
                        .FirstAsync();
                } 
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No channel with id " + id);
                }

                subscribers = await db.Subscribers
                    .Where(s => s.DeletedAt == null && data.Ids.Contains(s.Id))
                    .ToListAsync();
                
                foreach (var subscriber in subscribers)
                {
                    if (!channel.Subscribers.Contains(subscriber))
                    {
                        channel.Subscribers.Add(subscriber);
                    }
                }

                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(channel).Build<Channel>();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when adding subscribers to channel.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when adding subscribers to channel.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response AddSubscribers {{ id: {id}, SubscribeChannelDto: {JsonFormatter.ToString(data)} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> RemoveSubscribers(HttpRequest request, [FromServices]DataContext db, [FromRoute]int id, [FromBody]SubscribeChannelDto data)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request RemoveSubscribers {{ id: {id}, SubscribeChannelDto: {JsonFormatter.ToString(data)} }}");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                Channel channel;
                ICollection<Subscriber> subscribers;

                try
                {
                    channel = await db.Channels
                        .Where(c => c.DeletedAt == null && c.Id == id)
                        .Include(c => c.Subscribers.Where(s => s.DeletedAt == null))
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No channel with id " + id);
                }

                subscribers = await db.Subscribers
                    .Where(s => s.DeletedAt == null && data.Ids.Contains(s.Id))
                    .ToListAsync();

                foreach (var subscriber in subscribers)
                {
                    if (channel.Subscribers.Contains(subscriber))
                    {
                        channel.Subscribers.Remove(subscriber);
                    }
                }

                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(channel).Build<Channel>();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when removing subscribers from channel.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when removing subscribers from channel.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response RemoveSubscribers {{ id: {id}, SubscribeChannelDto: {JsonFormatter.ToString(data)} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }
    }
}
