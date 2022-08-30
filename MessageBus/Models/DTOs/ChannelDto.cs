using System.ComponentModel.DataAnnotations;

namespace MessageBus.Models.DTOs
{
    public class ChannelDto
    {
        [Required]
        public string Name { get; set; }

        public Channel ToChannel()
        {
            Channel channel = new Channel();
            channel.Name = Name;
            return channel;
        }
    }
}
