using MessageBus.Models;

namespace BackOfficeWeb.Interfaces
{
    public interface IMessageBusService
    {
        Task<ICollection<Channel>> GetAllChannelsAsync();
    }
}
