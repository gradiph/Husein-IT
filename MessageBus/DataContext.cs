using MessageBus.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageBus
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MessageSubscriber>().HasKey(table => new {
                table.MessageId,
                table.SubscriberId
            });
        }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageSubscriber> MessageSubscribers { get; set; }
    }
}
