using MessageBus.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageBus
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) { }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
