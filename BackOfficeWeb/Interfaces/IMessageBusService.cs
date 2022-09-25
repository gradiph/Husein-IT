using BackOfficeWeb.Models.MessageBus;

namespace BackOfficeWeb.Interfaces
{
    public interface IMessageBusService
    {
        /// <summary>
        /// Get all channels asynchronously.
        /// </summary>
        /// <exception cref="HttpRequestException">if API call doesn't return success response code</exception>
        /// <returns>All channels</returns>
        ValueTask<ICollection<Channel>> GetAllChannelsAsync();
        /// <summary>
        /// Get single channel asynchronously with loaded Subscribers and Messages.
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <exception cref="HttpRequestException">if API call doesn't return success response code</exception>
        /// <returns>Single channel</returns>
        ValueTask<Channel> GetChannelAsync(int id);
        /// <summary>
        /// Create channel asynchronously.
        /// </summary>
        /// <param name="data">Channel DTO</param>
        /// <exception cref="HttpRequestException">if API call doesn't return success response code</exception>
        /// <returns>Newly created channel</returns>
        ValueTask<Channel> CreateChannelAsync(ChannelDto data);
        /// <summary>
        /// Edit channel asynchronously.
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <param name="data">Channel DTO</param>
        /// <exception cref="HttpRequestException">if API call doesn't return success response code</exception>
        /// <returns>Updated channel</returns>
        ValueTask<Channel> UpdateChannelAsync(int id, ChannelDto data);
        /// <summary>
        /// Soft Delete channel asynchronously.
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <exception cref="HttpRequestException">if API call doesn't return success response code</exception>
        Task DeleteChannelAsync(int id);
        /// <summary>
        /// Restore soft deleted channel asynchronously.
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <exception cref="HttpRequestException">if API call doesn't return success response code</exception>
        /// <returns>Restored channel</returns>
        ValueTask<Channel> RestoreChannelAsync(int id);
        /// <summary>
        /// Permanent delete/destroy channel asynchronously.
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <exception cref="HttpRequestException">if API call doesn't return success response code</exception>
        Task DestroyChannelAsync(int id);
        /// <summary>
        /// Add subscriber(s) to channel asynchronously.
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <param name="data">SubscribeChannel DTO</param>
        /// <exception cref="HttpRequestException">if API call doesn't return success response code</exception>
        /// <returns>Updated channel</returns>
        ValueTask<Channel> AddSubscribersAsync(int id, SubscribeChannelDto data);
        /// <summary>
        /// Remove subscriber(s) to channel asynchronously.
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <param name="data">SubscribeChannel DTO</param>
        /// <exception cref="HttpRequestException">if API call doesn't return success response code</exception>
        /// <returns>Updated channel</returns>
        ValueTask<Channel> RemoveSubscriberAsync(int id, SubscribeChannelDto data);
    }
}
