using BackOfficeWeb.Models.MessageBus;

namespace BackOfficeWeb.Interfaces
{
    public interface IMessageBusService
    {
        ValueTask<ICollection<Channel>> GetAllChannelsAsync();
        ValueTask<Channel> GetChannelAsync(int id);
        ValueTask<Channel> CreateChannelAsync(ChannelDto data);
    }
}
