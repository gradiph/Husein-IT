using MessageBus.Models;
using MessageBus.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MessageBus.APIs
{
    public static class ChannelApi
    {
        public static void RegisterChannelApi(this WebApplication app)
        {
            app.MapGet("/channels", GetAllChannels);
            app.MapGet("/channels/{id}", GetChannel);
            app.MapPost("/channels", CreateChannel);
            app.MapPut("/channels/{id}", UpdateChannel);
            app.MapDelete("/channels/{id}", SoftDeleteChannel);
            app.MapPost("/channels/{id}/restore", RestoreChannel);
            app.MapDelete("/channels/{id}/force", DestroyChannel);
        }

        public async static Task<IResult> GetAllChannels(HttpRequest request, DataContext db)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream, "Request GetAllChannels");

            var statusCode = 200;
            var message = "success";
            object response = null;
            try
            {
                List<Channel> channels = await db.Channels
                    .Where(c => c.DeletedAt == null)
                    .ToListAsync();

                response = new JsonResponseBuilder(channels).Build< List<Channel>>();
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when retrieving all channels.", e);
                statusCode = 500;
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

            var statusCode = 200;
            var message = "success";
            object response = null;
            try
            {
                Channel channel = await db.Channels
                    .Where(c => c.DeletedAt == null && c.Id == id)
                    .Include(c => c.Subscribers.Where(s => s.DeletedAt == null))
                    .Include(c => c.Messages)
                    .FirstAsync();

                response = new JsonResponseBuilder(channel).Build<Channel>();
            } catch (InvalidOperationException)
            {
                statusCode = 422;
                message = "No channel with id " + id;
            } catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when retrieving channel.", e);
                statusCode = 500;
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

            var statusCode = 201;
            var message = "success";
            object response = null;
            try
            {
                var channel = channelDto.ToChannel();

                db.Channels.Add(channel);
                await db.SaveChangesAsync();

                message = $"/channel/{channel.Id}";

                response = new JsonResponseBuilder(channel).Build<Channel>();
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when creating channel.", e);
                statusCode = 500;
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

            var statusCode = 200;
            var message = "success";
            object response = null;
            try
            {
                Channel channel = await db.Channels
                    .Where(c => c.DeletedAt == null && c.Id == id)
                    .FirstAsync();
                
                channel.Name = channelDto.Name;

                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(channel).Build<Channel>();
            }
            catch (InvalidOperationException)
            {
                statusCode = 422;
                message = "No channel with id " + id;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when updating channel.", e);
                statusCode = 500;
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

            var statusCode = 204;
            var message = "success";
            object response = null;
            try
            {
                Channel channel = await db.Channels
                    .Where(c => c.DeletedAt == null && c.Id == id)
                    .FirstAsync();

                channel.DeletedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                statusCode = 422;
                message = "No channel with id " + id;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when soft deleting channel.", e);
                statusCode = 500;
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

            var statusCode = 200;
            var message = "success";
            object response = null;
            try
            {
                Channel channel = await db.Channels
                    .Where(c => c.DeletedAt != null && c.Id == id)
                    .FirstAsync();

                channel.DeletedAt = null;
                await db.SaveChangesAsync();

                response = new JsonResponseBuilder(channel).Build<Channel>();
            }
            catch (InvalidOperationException)
            {
                statusCode = 422;
                message = "No channel with id " + id;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when restoring channel.", e);
                statusCode = 500;
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

            var statusCode = 204;
            var message = "success";
            object response = null;
            try
            {
                Channel channel = await db.Channels
                    .Where(c => c.DeletedAt != null && c.Id == id)
                    .FirstAsync();

                db.Channels.Remove(channel);
                await db.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                statusCode = 422;
                message = "No channel with id " + id;
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(db, LogType.Error, "Error when destroying channel.", e);
                statusCode = 500;
                message = e.Message;
            }

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response DestroyChannel {{ id: {id} }} " +
                $"[{statusCode}]: {JsonFormatter.ToString(response)}");
            return new ApiResponseBuilder(request, statusCode, response, message).Build();
        }
    }
}
