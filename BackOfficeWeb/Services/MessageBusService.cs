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
            
            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            using var httpResponse = await httpClient.GetAsync("/channels");
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            ICollection<Channel> result = JsonFormatter.ParseString<ICollection<Channel>>(content);
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
            return result;
        }
        public async ValueTask<Channel> GetChannelAsync(int id)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"GetChannelAsync start with id {id}.");

            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            using var httpResponse = await httpClient.GetAsync($"/channels/{id}");
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            Channel result = JsonFormatter.ParseString<Channel>(content);
                
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"GetChannelAsync result: {JsonFormatter.ToString(result)}.");
            return result;
        }

        public async ValueTask<Channel> CreateChannelAsync(ChannelDto data)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"CreateChannelAsync start with {nameof(ChannelDto)} {JsonFormatter.ToString(data)}.");

            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            using var postData = new StringContent(JsonFormatter.ToString(data), Encoding.UTF8, Application.Json);
            using var httpResponse = await httpClient.PostAsync($"/channels", postData);
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            Channel result = JsonFormatter.ParseString<Channel>(content);

            LogWriter.Instance.LogAsync(this, LogType.Trace, $"CreateChannelAsync result: {JsonFormatter.ToString(result)}.");
            return result;
        }

        public async ValueTask<Channel> UpdateChannelAsync(int id, ChannelDto data)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"UpdateChannelAsync start with id {id} and {nameof(ChannelDto)} {JsonFormatter.ToString(data)}.");

            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            using var postData = new StringContent(JsonFormatter.ToString(data), Encoding.UTF8, Application.Json);
            using var httpResponse = await httpClient.PutAsync($"/channels/{id}", postData);
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            Channel result = JsonFormatter.ParseString<Channel>(content);

            LogWriter.Instance.LogAsync(this, LogType.Trace, $"UpdateChannelAsync result: {JsonFormatter.ToString(result)}.");
            return result;
        }

        public async Task DeleteChannelAsync(int id)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"DeleteChannelAsync start with id {id}.");

            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            using var httpResponse = await httpClient.DeleteAsync($"/channels/{id}");
            httpResponse.EnsureSuccessStatusCode();

            LogWriter.Instance.LogAsync(this, LogType.Trace, $"DeleteChannelAsync result: success.");
        }

        public async ValueTask<Channel> RestoreChannelAsync(int id)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"RestoreChannelAsync start with id {id}.");

            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            using var httpResponse = await httpClient.PostAsync($"/channels/{id}", null);
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            Channel result = JsonFormatter.ParseString<Channel>(content);

            LogWriter.Instance.LogAsync(this, LogType.Trace, $"RestoreChannelAsync result: {JsonFormatter.ToString(result)}.");
            return result;
        }

        public async Task DestroyChannelAsync(int id)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"DestroyChannelAsync start with id {id}.");

            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            using var httpResponse = await httpClient.DeleteAsync($"/channels/{id}/force");
            httpResponse.EnsureSuccessStatusCode();

            LogWriter.Instance.LogAsync(this, LogType.Trace, $"DestroyChannelAsync result: success.");
        }

        public async ValueTask<Channel> AddSubscribersAsync(int id, SubscribeChannelDto data)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"AddSubscribersAsync start with id {id} and {nameof(SubscribeChannelDto)} {JsonFormatter.ToString(data)}.");

            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            using var postData = new StringContent(JsonFormatter.ToString(data), Encoding.UTF8, Application.Json);
            using var httpResponse = await httpClient.PostAsync($"/channels/{id}/subscribers", postData);
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            Channel result = JsonFormatter.ParseString<Channel>(content);

            LogWriter.Instance.LogAsync(this, LogType.Trace, $"AddSubscribersAsync result: {JsonFormatter.ToString(result)}.");
            return result;
        }

        public async ValueTask<Channel> RemoveSubscriberAsync(int id, SubscribeChannelDto data)
        {
            LogWriter.Instance.LogAsync(this, LogType.Trace, $"RemoveSubscriberAsync start with id {id} and {nameof(SubscribeChannelDto)} {JsonFormatter.ToString(data)}.");

            using var httpClient = _httpClientFactory.CreateClient(ServiceRegister.HTTP_CLIENT_MESSAGE_BUS);
            using var postData = new StringContent(JsonFormatter.ToString(data), Encoding.UTF8, Application.Json);
            using var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"/channels/{id}/subscribers"),
                Content = postData
            };
            using var httpResponse = await httpClient.SendAsync(httpRequest);
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            Channel result = JsonFormatter.ParseString<Channel>(content);

            LogWriter.Instance.LogAsync(this, LogType.Trace, $"RemoveSubscriberAsync result: {JsonFormatter.ToString(result)}.");
            return result;
        }
    }
}
