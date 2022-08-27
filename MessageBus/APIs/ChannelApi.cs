using MessageBus.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommonJson;

namespace MessageBus.APIs
{
    public static class ChannelApi
    {
        public static void RegisterChannelApi(this WebApplication app)
        {
            app.MapGet("/channels", GetAllChannels);
        }

        public async static Task<List<Channel>> GetAllChannels(DataContext db)
        {
            var channels = await db.Channels.ToListAsync();
            List<Channel> channelsList = new JsonResponseBuilder(channels).Build<List<Channel>>();
            return channelsList;
        }

        public async static Task<Channel> GetChannel(DataContext db)
        {
            var channel = await db.Channels.FirstOrDefaultAsync();
            return channel;
        }
    }
}
