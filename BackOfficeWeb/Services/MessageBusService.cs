using BackOfficeWeb.Interfaces;
using BackOfficeWeb.Models.MessageBus;

namespace BackOfficeWeb.Services
{
    public class MessageBusService: IMessageBusService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MessageBusService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async ValueTask<ICollection<Channel>> GetAllChannelsAsync()
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"GetAllChannelsAsync start.");
            ICollection<Channel> result = new List<Channel>();
            
            var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            var httpResponse = await httpClient.GetAsync("/channels");
            
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = await httpResponse.Content.ReadAsStringAsync();
                result = JsonFormatter.ParseString<ICollection<Channel>>(content);
                foreach (var item in result)
                {
                    var channel = await GetChannelAsync(item.id);
                    if (channel != null)
                    {
                        item.subscribersCount = channel.subscribers.Count();
                        item.messagesCount = channel.messages.Count();
                    }
                }
                LogWriter.Instance.LogAsync(this, LogType.Trace, $"GetAllChannelsAsync result: {JsonFormatter.ToString(result)}.");
            }
            httpResponse.Dispose();
            return result;
        }

        public async ValueTask<Channel> GetChannelAsync(int id)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"GetChannelAsync start.");
            Channel result = null;

            var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            var httpResponse = await httpClient.GetAsync($"/channels/{id}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = await httpResponse.Content.ReadAsStringAsync();
                result = JsonFormatter.ParseString<Channel>(content);
                
                LogWriter.Instance.LogAsync(this, LogType.Trace, $"GetChannelAsync result: {JsonFormatter.ToString(result)}.");
            }
            httpResponse.Dispose();
            return result;
        }
    }
}
