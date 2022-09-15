using BackOfficeWeb.Models.MessageBus;

namespace BackOfficeWeb.Interfaces
{
    public interface IMessageBusService
    {
        ValueTask<ICollection<Channel>> GetAllChannelsAsync();
        ValueTask<Channel> GetChannelAsync(int id);
        ValueTask<Channel> CreateChannelAsync(ChannelDto data);
        ValueTask<Channel> UpdateChannelAsync(int id, ChannelDto data);
        Task DeleteChannelAsync(int id);
        ValueTask<Channel> RestoreChannelAsync(int id);
        Task DestroyChannelAsync(int id);
        ValueTask<Channel> AddSubscribersAsync(int id, SubscribeChannelDto data);
        ValueTask<Channel> RemoveSubscriberAsync(int id, SubscribeChannelDto data);
    }
}
