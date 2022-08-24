using Microsoft.EntityFrameworkCore;

namespace MessageBus.APIs
{
    public static class ChannelApi
    {
        public static void RegisterChannelApi(this WebApplication app)
        {
            app.MapGet("/channels", async (DataContext db) =>
            {
                var channels = await db.Channels
                    .Include(c => c.Subscribers)
                    .Include(c => c.Messages)
                    .ToListAsync();
                return Results.Ok(channels);
            });
        }
    }
}
