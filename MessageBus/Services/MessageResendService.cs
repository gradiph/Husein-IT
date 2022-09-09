using MessageBus.Interfaces;
using MessageBus.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageBus.Services
{
    public class MessageResendService : BackgroundService
    {
        private readonly IBackgroundMessageQueue _queue;
        private readonly IServiceProvider _serviceProvider;

        public MessageResendService(IBackgroundMessageQueue queue, IServiceProvider serviceProvider)
        {
            LogWriter.Instance.LogAsync(queue, LogType.Trace, $"Initiating {nameof(MessageResendService)}...");
            _queue = queue;
            _serviceProvider = serviceProvider;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"{nameof(MessageResendService)} is starting...");
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"{nameof(MessageResendService)} is stopping...");
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"{nameof(MessageResendService)} is executing...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var undeliveredMessages = await GetUndeliveredMessageAsync();

                    foreach (var messageSubscriber in undeliveredMessages)
                    {
                        await _queue.EnqueueAsync(messageSubscriber);

                        LogWriter.Instance.LogAsync(this, LogType.Trace, $"Successfully put {messageSubscriber} to queue.");
                    }
                }
                catch (Exception e)
                {
                    LogWriter.Instance.LogAsync(this, LogType.Error, $"Unhandled exception while executing {nameof(MessageResendService)}.", e);
                }
                
                await Task.Delay(1000 * 60 * 5);
            }
        }

        protected async ValueTask<ICollection<MessageSubscriber>> GetUndeliveredMessageAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            ICollection<MessageSubscriber> undeliveredMessages = new List<MessageSubscriber>();

            try
            {
                LogWriter.Instance.LogAsync(this, LogType.Trace, $"Retrieving undelivered messages from db.");

                undeliveredMessages = await db.MessageSubscribers
                    .Where(ms => ms.SentAt == null)
                    .Include(ms => ms.Subscriber)
                    .Include(ms => ms.Message)
                    .ToListAsync();

                LogWriter.Instance.LogAsync(this, LogType.Trace, $"Found {undeliveredMessages.Count} undelivered messages.");
            }
            catch (InvalidOperationException)
            {
                LogWriter.Instance.LogAsync(this, LogType.Trace, $"Found 0 undelivered messages.");
            }
            return undeliveredMessages;
        }
    }
}
