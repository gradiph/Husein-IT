using MessageBus.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MessageBus.APIs
{
    public static class SubscriberApi
    {
        public static void RegisterSubscriberApi(this WebApplication app)
        {
            app.MapGet("/subscribers", GetAllSubscriber);
            app.MapGet("/subscribers/{id}", GetSubscriber);
            app.MapPost("/subscribers", CreateSubscriber);
            app.MapPut("/subscribers/{id}", UpdateSubscriber);
            app.MapDelete("/subscribers/{id}", SoftDeleteSubscriber);
            app.MapPost("/subscribers/{id}/restore", RestoreSubscriber);
            app.MapDelete("/subscribers/{id}/force", DestroySubscriber);
        }

        public async static Task<IResult> GetAllSubscriber(HttpRequest request, DataContext db)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream, "Request GetAllSubscriber");

            var statusCode = 200;
            var message = "success";
            object response = null;
            try
            {
                List<Subscriber> subscribers = await db.Subscribers
                    .Where(c => c.DeletedAt == null)
                    .ToListAsync();

                response = new JsonResponseBuilder(subscribers).Build<List<Subscriber>>();
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when retrieving all subscribers.", e);
                statusCode = 500;
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

            var statusCode = 200;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber = await db.Subscribers
                    .Where(c => c.DeletedAt == null && c.Id == id)
                    .Include(c => c.Channels.Where(s => s.DeletedAt == null))
                    .Include(c => c.Messages)
                    .FirstAsync();

                response = new JsonResponseBuilder(subscriber).Build<Subscriber>();
            }
            catch (InvalidOperationException)
            {
                statusCode = 422;
                message = "No subscriber with id " + id;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when retrieving subscriber.", e);
                statusCode = 500;
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

            var statusCode = 201;
            var message = "success";
            object response = null;
            try
            {
                var subscriber = subscriberDto.ToSubscriber();

                db.Subscribers.Add(subscriber);
                await db.SaveChangesAsync();

                message = $"/subscriber/{subscriber.Id}";

                response = new JsonResponseBuilder(subscriber).Build<Channel>();
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when creating subscriber.", e);
                statusCode = 500;
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

            var statusCode = 200;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber = await db.Subscribers
                    .Where(c => c.DeletedAt == null && c.Id == id)
                    .FirstAsync();

                subscriber.Name = subscriberDto.Name;

                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(subscriber).Build<Subscriber>();
            }
            catch (InvalidOperationException)
            {
                statusCode = 422;
                message = "No subscriber with id " + id;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when updating subscriber.", e);
                statusCode = 500;
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

            var statusCode = 204;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber = await db.Subscribers
                    .Where(c => c.DeletedAt == null && c.Id == id)
                    .FirstAsync();

                subscriber.DeletedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                statusCode = 422;
                message = "No subscriber with id " + id;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when soft deleting subscriber.", e);
                statusCode = 500;
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

            var statusCode = 200;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber = await db.Subscribers
                    .Where(c => c.DeletedAt != null && c.Id == id)
                    .FirstAsync();

                subscriber.DeletedAt = null;
                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(subscriber).Build<Subscriber>();
            }
            catch (InvalidOperationException)
            {
                statusCode = 422;
                message = "No subscriber with id " + id;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when restoring subscriber.", e);
                statusCode = 500;
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

            var statusCode = 204;
            var message = "success";
            object response = null;
            try
            {
                Subscriber subscriber = await db.Subscribers
                    .Where(c => c.DeletedAt != null && c.Id == id)
                    .FirstAsync();

                db.Subscribers.Remove(subscriber);
                await db.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                statusCode = 422;
                message = "No subscriber with id " + id;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when destroying subscriber.", e);
                statusCode = 500;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response DestroySubscriber {{ id: {id} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }
    }
}
