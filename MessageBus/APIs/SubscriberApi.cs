using Microsoft.EntityFrameworkCore;

namespace MessageBus.APIs
{
    public static class SubscriberApi
    {
        public static void RegisterSubscriberApi(this WebApplication app)
        {
            app.MapGet("/subscribers", async (DataContext db) =>
            {
                var subscribers = await db.Subscribers
                    .Include(s => s.Channels)
                    .Include(s => s.Messages)
                    .ToListAsync();
                return Results.Ok(subscribers);
            });
        }
    }
}
