using MessageBus.Interfaces;
using Models = MessageBus.Models;
using System.Threading.Channels;

namespace MessageBus.Services
{
    public class MessageQueueService : IBackgroundMessageQueue
    {
        private readonly Channel<Models.MessageSubscriber> _queue;

        public MessageQueueService(int capacity)
        {
            LogWriter.Instance.LogAsync(new Models.Message(), LogType.Trace, $"Initiating {nameof(MessageQueueService)}...");
            BoundedChannelOptions options = new(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<Models.MessageSubscriber>(options);
        }

        public async Task EnqueueAsync(Models.MessageSubscriber messageSubscriber)
        {
            if (messageSubscriber is null)
            {
                LogWriter.Instance.LogAsync(this, LogType.Trace, $"Cannot enqueue empty {nameof(Models.MessageSubscriber)}.");
                throw new ArgumentNullException(nameof(messageSubscriber));
            }

            LogWriter.Instance.LogAsync(this, LogType.Trace, $"Enqueuing {nameof(Models.MessageSubscriber)} {messageSubscriber}...");
            await _queue.Writer.WriteAsync(messageSubscriber);
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"Successfully enqueued {messageSubscriber}. Current queue size: {_queue.Reader.Count}");
        }

        public async ValueTask<Models.MessageSubscriber> DequeueAsync(CancellationToken cancellationToken)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"Dequeuing message from queue...");
            Models.MessageSubscriber messageSubscriber = await _queue.Reader.ReadAsync(cancellationToken);
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"Got {nameof(Models.MessageSubscriber)} from queue: {messageSubscriber}.");
            return messageSubscriber;
        }
    }
}
