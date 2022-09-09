using MessageBus.Interfaces;
using MessageBus.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace MessageBus.Services
{
    public class MessageSenderService : BackgroundService
    {
        private readonly IBackgroundMessageQueue _queue;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceProvider _serviceProvider;

        public MessageSenderService(IBackgroundMessageQueue queue, IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider)
        {
            LogWriter.Instance.LogAsync(queue, LogType.Trace, $"Initiating {nameof(MessageSenderService)}...");
            _queue = queue;
            _httpClientFactory = httpClientFactory;
            _serviceProvider = serviceProvider;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"{nameof(MessageSenderService)} is starting...");
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"{nameof(MessageSenderService)} is stopping...");
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"{nameof(MessageSenderService)} is executing...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    MessageSubscriber messageSubscriber = await _queue.DequeueAsync(stoppingToken);

                    await SendMessage(messageSubscriber);
                }
                catch (OperationCanceledException e)
                {
                    LogWriter.Instance.LogAsync(this, LogType.Trace, $"{stoppingToken} was signalled.", e);
                }
                catch (Exception e)
                {
                    LogWriter.Instance.LogAsync(this, LogType.Error, $"Unhandled exception while executing {nameof(MessageSenderService)}.", e);
                }
            }
        }

        protected async Task SendMessage(MessageSubscriber messageSubscriber)
        {
            LogWriter.Instance.LogAsync(messageSubscriber, LogType.Trace, $"Sending {nameof(MessageSubscriber)} {messageSubscriber}...");

            using var httpClient = _httpClientFactory.CreateClient();
            var data = new StringContent(messageSubscriber.Message.Data, Encoding.UTF8, Application.Json);
            var url = messageSubscriber.Subscriber.Url;

            LogWriter.Instance.LogAsync(messageSubscriber, LogType.Trace, $"Sending {data} to {url}...");
            using var httpResponseMessage = await httpClient.PostAsync(url, data);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                LogWriter.Instance.LogAsync(messageSubscriber, LogType.Trace, $"{nameof(messageSubscriber.Message)} {messageSubscriber.Message} is sent to {url}.");

                await UpdateArrived(messageSubscriber);
            }
            else
            {
                LogWriter.Instance.LogAsync(messageSubscriber, LogType.Trace, $"Failed to send {nameof(messageSubscriber.Message)} {messageSubscriber.Message} to {url}. " +
                    $"Got response code: {httpResponseMessage.StatusCode}.");
            }
        }

        private async Task UpdateArrived(MessageSubscriber messageSubscriber)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            try
            {
                var messageSubscriberDb = await db.MessageSubscribers
                    .Where(ms => ms.SubscriberId == messageSubscriber.SubscriberId && ms.MessageId == messageSubscriber.MessageId)
                    .FirstAsync();
                messageSubscriberDb.SentAt = DateTime.Now;
                await db.SaveChangesAsync();
                LogWriter.Instance.LogAsync(messageSubscriber, LogType.Trace, $"Update arrived on {messageSubscriber} is successful.");
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogAsync(messageSubscriber, LogType.Trace, $"Failed to update arrived on {messageSubscriber}", e);
            }
        }
    }
}
