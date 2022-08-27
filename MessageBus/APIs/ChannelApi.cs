using MessageBus.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            var option = new JsonSerializerOptions { 
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(channels, option);
            List<Channel> channelsList = JsonSerializer.Deserialize<List<Channel>>(json);
            return channelsList;
        }
    }
}
