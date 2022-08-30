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

            Channel channel;
            try
            {
                channel = await db.Channels
                    .Where(c => c.Id == id)
                    .Include(c => c.Subscribers)
                    .Include(c => c.Messages)
                    .FirstAsync();
            }
            catch (Exception)
            {
                var message = "No channel with id " + id;
                LogWriter.Instance.LogAsync(db, LogType.Stream, 
                    $"Response GetChannel {{ id: {id} }} [400]: {message}");
                return Results.BadRequest(message);
            }
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
    }
}
