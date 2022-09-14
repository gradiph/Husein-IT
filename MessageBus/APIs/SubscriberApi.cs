using MessageBus.Models;
using MessageBus.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessageBus.APIs
{
    public static class SubscriberApi
    {
        public static void RegisterSubscriberApi(this WebApplication app)
        {
            app.MapGet("/subscribers", GetAllSubscriber)
                .Produces<ICollection<Subscriber>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapGet("/subscribers/{id}", GetSubscriber)
                .Produces<Subscriber>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapPost("/subscribers", CreateSubscriber)
                .Produces<Subscriber>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapPut("/subscribers/{id}", UpdateSubscriber)
                .Produces<Subscriber>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapDelete("/subscribers/{id}", SoftDeleteSubscriber)
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapPost("/subscribers/{id}/restore", RestoreSubscriber)
                .Produces<Subscriber>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapDelete("/subscribers/{id}/force", DestroySubscriber)
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapPost("/subscribers/{id}/channels", AddChannels)
                .Produces<Subscriber>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);

            app.MapDelete("/subscribers/{id}/channels", RemoveChannels)
                .Produces<Subscriber>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }

        public async static Task<IResult> GetAllSubscriber(HttpRequest request, DataContext db)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream, "Request GetAllSubscriber");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                List<Subscriber> subscribers = await db.Subscribers
                    .Where(s => s.DeletedAt == null)
                    .ToListAsync();

                response = new JsonResponseBuilder(subscribers).Build<List<Subscriber>>();
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when retrieving all subscribers.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response GetAllSubscriber [{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> GetSubscriber(HttpRequest request, DataContext db, int id)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request GetSubscriber {{ id: {id} }}");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber;
                try
                {
                    subscriber = await db.Subscribers
                        .Where(s => s.DeletedAt == null && s.Id == id)
                        .Include(s => s.Channels.Where(c => c.DeletedAt == null))
                        .Include(s => s.Messages)
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No subscriber with id " + id);
                }

                response = new JsonResponseBuilder(subscriber).Build<Subscriber>();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when retrieving subscriber.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when retrieving subscriber.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response GetSubscriber {{ id: {id} }} [{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> CreateSubscriber(HttpRequest request, DataContext db, SubscriberDto subscriberDto)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request CreateSubscriber {{ SubscriberDto: {JsonFormatter.ToString(subscriberDto)} }}");

            var statusCode = StatusCodes.Status201Created;
            var message = "success";
            object response = null;
            try
            {
                var subscriber = subscriberDto.ToSubscriber();

                db.Subscribers.Add(subscriber);
                await db.SaveChangesAsync();

                message = $"/subscribers/{subscriber.Id}";
                response = new JsonResponseBuilder(subscriber).Build<Subscriber>();
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when creating subscriber.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response CreateSubscriber {{ SubscriberDto: {JsonFormatter.ToString(subscriberDto)} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}"
            );
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> UpdateSubscriber(HttpRequest request, DataContext db, int id, SubscriberDto subscriberDto)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request UpdateChannel {{ id: {id}, SubscriberDto: {JsonFormatter.ToString(subscriberDto)} }}");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber;
                try
                {
                    subscriber = await db.Subscribers
                        .Where(s => s.DeletedAt != null && s.Id == id)
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No subscriber with id " + id);
                }

                subscriber.Name = subscriberDto.Name;

                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(subscriber).Build<Subscriber>();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when updating subscriber.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when updating subscriber.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response UpdateChannel {{ id: {id}, SubscriberDto: {JsonFormatter.ToString(subscriberDto)} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> SoftDeleteSubscriber(HttpRequest request, DataContext db, int id)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request SoftDeleteSubscriber {{ id: {id} }}");

            var statusCode = StatusCodes.Status204NoContent;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber;
                try
                {
                    subscriber = await db.Subscribers
                        .Where(s => s.DeletedAt != null && s.Id == id)
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No subscriber with id " + id);
                }

                subscriber.DeletedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when soft deleting subscriber.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when soft deleting subscriber.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response SoftDeleteSubscriber {{ id: {id} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> RestoreSubscriber(HttpRequest request, DataContext db, int id)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request RestoreSubscriber {{ id: {id} }}");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber;
                try
                {
                    subscriber = await db.Subscribers
                        .Where(s => s.DeletedAt != null && s.Id == id)
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No subscriber with id " + id);
                }

                subscriber.DeletedAt = null;
                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(subscriber).Build<Subscriber>();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when restoring subscriber.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when restoring subscriber.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response RestoreSubscriber {{ id: {id} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> DestroySubscriber(HttpRequest request, DataContext db, int id)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request DestroySubscriber {{ id: {id} }}");

            var statusCode = StatusCodes.Status204NoContent;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber;
                try
                {
                    subscriber = await db.Subscribers
                        .Where(s => s.DeletedAt != null && s.Id == id)
                        .FirstAsync();
                } catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No subscriber with id " + id);
                }

                db.Subscribers.Remove(subscriber);
                await db.SaveChangesAsync();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when destroying subscriber.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when destroying subscriber.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response DestroySubscriber {{ id: {id} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> AddChannels(HttpRequest request, DataContext db, int id, SubscribeChannelDto data)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request AddChannels {{ id: {id}, SubscribeChannelDto: {JsonFormatter.ToString(data)} }}");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber;
                ICollection<Channel> channels;

                try
                {
                    subscriber = await db.Subscribers
                        .Where(s => s.DeletedAt == null && s.Id == id)
                        .Include(s => s.Channels.Where(c => c.DeletedAt == null))
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No subscriber with id " + id);
                }

                channels = await db.Channels
                    .Where(s => s.DeletedAt == null && data.Ids.Contains(s.Id))
                    .ToListAsync();

                foreach (var channel in channels)
                {
                    if (!subscriber.Channels.Contains(channel))
                    {
                        subscriber.Channels.Add(channel);
                    }
                }

                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(subscriber).Build<Subscriber>();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when adding channels to subscriber.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when adding channels to subscriber.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response AddChannels {{ id: {id}, SubscribeChannelDto: {JsonFormatter.ToString(data)} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }

        public async static Task<IResult> RemoveChannels(HttpRequest request, [FromServices] DataContext db, [FromRoute] int id, [FromBody] SubscribeChannelDto data)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request RemoveChannels {{ id: {id}, SubscribeChannelDto: {JsonFormatter.ToString(data)} }}");

            var statusCode = StatusCodes.Status200OK;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber;
                ICollection<Channel> channels;

                try
                {
                    subscriber = await db.Subscribers
                        .Where(s => s.DeletedAt == null && s.Id == id)
                        .Include(s => s.Channels.Where(c => c.DeletedAt == null))
                        .FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("No subscriber with id " + id);
                }

                channels = await db.Channels
                    .Where(c => c.DeletedAt == null && data.Ids.Contains(c.Id))
                    .ToListAsync();

                foreach (var channel in channels)
                {
                    if (subscriber.Channels.Contains(channel))
                    {
                        subscriber.Channels.Remove(channel);
                    }
                }

                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(subscriber).Build<Subscriber>();
            }
            catch (InvalidOperationException e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Trace, "Unprocessable when removing channels from subscriber.", e);
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = e.Message;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when removing channels from subscriber.", e);
                statusCode = StatusCodes.Status500InternalServerError;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response RemoveChannels {{ id: {id}, SubscribeChannelDto: {JsonFormatter.ToString(data)} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }
    }
}
