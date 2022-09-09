using MessageBus.Models;

namespace MessageBus.Interfaces
{
    public interface IBackgroundMessageQueue
    {
        Task EnqueueAsync(MessageSubscriber workItem);
        ValueTask<MessageSubscriber> DequeueAsync(CancellationToken cancellationToken);
    }
}
