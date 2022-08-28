using MessageBus.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommonJson;
using Microsoft.AspNetCore.Mvc;

namespace MessageBus.APIs
{
    public static class ChannelApi
    {
        public static void RegisterChannelApi(this WebApplication app)
        {
            app.MapGet("/channels", GetAllChannels);
            app.MapGet("/channel", GetChannel);
        }

        public async static Task<IResult> GetAllChannels(DataContext db)
        {
            List<Channel> channels = await db.Channels.ToListAsync();
            List<Channel> response = new JsonResponseBuilder(channels).Build<List<Channel>>();
            return Results.Ok(response);
        }

        public async static Task<IResult> GetChannel(DataContext db, int id)
        {
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
                return Results.BadRequest("No channel with id " + id);
            }
            Channel response = new JsonResponseBuilder(channel).Build<Channel>();
            return Results.Ok(response);
        }
    }
}
