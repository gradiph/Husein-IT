using BackOfficeWeb.Interfaces;
using CommonJson;
using MessageBus.Models;

namespace BackOfficeWeb.Services
{
    public class MessageBusService: IMessageBusService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MessageBusService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ICollection<Channel>> GetAllChannelsAsync()
        {
            ICollection<Channel> result = new List<Channel>();
            
            var httpClient = _httpClientFactory.CreateClient("MessageBus");
            var httpResponse = await httpClient.GetAsync("channels");
            
            if (httpResponse.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponse.Content.ReadAsStreamAsync();
                result = JsonFormatter.ParseStream<ICollection<Channel>>(contentStream);
            }
            httpResponse.Dispose();
            return result;
        }
    }
}
