namespace BackOfficeWeb.Models.MessageBus
{
    public class ChannelViewModel
    {
        public IEnumerable<Channel> Channels { get; set; } = new List<Channel>();
        public Channel Channel { get; set; } = new Channel();
    }
}
