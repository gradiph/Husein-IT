using CommonJson;
using CommonLog;
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
            app.MapGet("/channel/{id}", GetChannel);
            app.MapPost("/channels", CreateChannel);
            app.MapPut("/channel/{id}", UpdateChannel);
            app.MapDelete("/channel/{id}", DeleteChannel);
        }

        public async static Task<IResult> GetAllChannels(DataContext db)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream, 
                $"Request GetAllChannels");

            List<Channel> channels = await db.Channels.ToListAsync();
            List<Channel> response = new JsonResponseBuilder(channels).Build<List<Channel>>();

            LogWriter.Instance.LogAsync(db, LogType.Stream, 
                $"Response GetAllChannels [200]: {JsonFormatter.ToString(response)}");
            return Results.Ok(response);
        }

        public async static Task<IResult> GetChannel(DataContext db, int id)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream, 
                $"Request GetChannel {{ id: {id} }}");

            Channel channel = await db.Channels.FindAsync(id);
            if (channel == null)
            {
                var message = "No channel with id " + id;
                LogWriter.Instance.LogAsync(db, LogType.Stream, 
                    $"Response GetChannel {{ id: {id} }} " +
                    $"[422]: " +
                    $"{message}");
                return Results.UnprocessableEntity(message);
            }
            await db.Entry(channel).Collection(c => c.Subscribers).LoadAsync();
            await db.Entry(channel).Collection(c => c.Messages).LoadAsync();

            Channel response = new JsonResponseBuilder(channel).Build<Channel>();
            LogWriter.Instance.LogAsync(db, LogType.Stream, 
                $"Response GetChannel {{ id: {id} }} [200]: {JsonFormatter.ToString(response)}");
            return Results.Ok(response);
        }

        public async static Task<IResult> CreateChannel(DataContext db, ChannelDto channelDto)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request CreateChannel {{ ChannelDto: {JsonFormatter.ToString(channelDto)} }}");

            var channel = channelDto.ToChannel();

            db.Channels.Add(channel);
            await db.SaveChangesAsync();

            var url = $"/channel/{channel.Id}";

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response CreateChannel {{ ChannelDto: {JsonFormatter.ToString(channelDto)} }} " +
                $"[201]: " +
                $"{{ url: {url}, channel: {JsonFormatter.ToString(channel)} }}"
            );
            return Results.Created(url, channel);
        }

        public async static Task<IResult> UpdateChannel(DataContext db, int id, ChannelDto channelDto)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request UpdateChannel {{ id: {id}, ChannelDto: {JsonFormatter.ToString(channelDto)} }}");

            Channel channel = await db.Channels.FindAsync(id);
            if (channel == null)
            {
                var message = "No Channel with id " + id;
                LogWriter.Instance.LogAsync(db, LogType.Stream,
                    $"Response UpdateChannel {{ id: {id}, ChannelDto: {JsonFormatter.ToString(channelDto)} }} " +
                    $"[422]: " +
                    $"{message}");
                return Results.UnprocessableEntity(message);
            }

            channel.Name = channelDto.Name;

            await db.SaveChangesAsync();

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response UpdateChannel {{ id: {id}, ChannelDto: {JsonFormatter.ToString(channelDto)} }} " +
                $"[200]: " +
                $"{{ channel: {JsonFormatter.ToString(channel)} }}"
            );
            return Results.Ok(channel);
        }

        public async static Task<IResult> DeleteChannel(DataContext db, int id)
        {
            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Request DeleteChannel {{ id: {id} }}");

            Channel channel = await db.Channels.FindAsync(id);
            if (channel == null)
            {
                var message = "No Channel with id " + id;
                LogWriter.Instance.LogAsync(db, LogType.Stream,
                    $"Response DeleteChannel {{ id: {id} }} " +
                    $"[422]: " +
                    $"{message}");
                return Results.UnprocessableEntity(message);
            }

            db.Channels.Remove(channel);
            await db.SaveChangesAsync();

            LogWriter.Instance.LogAsync(db, LogType.Stream,
                $"Response DeleteChannel {{ id: {id} }} " +
                $"[204]: "
            );
            return Results.NoContent();
        }
    }
}
