using BackOfficeWeb.Interfaces;
using BackOfficeWeb.Models.MessageBus;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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
            
            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
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
            return result;
        }

        public async ValueTask<Channel> GetChannelAsync(int id)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"GetChannelAsync start.");
            Channel? result = null;

            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            var httpResponse = await httpClient.GetAsync($"/channels/{id}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = await httpResponse.Content.ReadAsStringAsync();
                result = JsonFormatter.ParseString<Channel>(content);
                
                LogWriter.Instance.LogAsync(this, LogType.Trace, $"GetChannelAsync result: {JsonFormatter.ToString(result)}.");
            }
            return result;
        }

        public async ValueTask<Channel> CreateChannelAsync(ChannelDto data)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"CreateChannelAsync start.");
            Channel? result = null;

            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            var postData = new StringContent(JsonFormatter.ToString(data), Encoding.UTF8, Application.Json);
            var httpResponse = await httpClient.PostAsync($"/channels", postData);

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = await httpResponse.Content.ReadAsStringAsync();
                result = JsonFormatter.ParseString<Channel>(content);

                LogWriter.Instance.LogAsync(this, LogType.Trace, $"CreateChannelAsync result: {JsonFormatter.ToString(result)}.");
            }
            return result;
        }
    }
}
