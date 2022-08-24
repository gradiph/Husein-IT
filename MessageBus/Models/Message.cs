using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MessageBus.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string PublisherName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public int ChannelId { get; set; }
        [JsonIgnore]
        public virtual Channel Channel { get; set; }
        [JsonIgnore]
        public virtual ICollection<MessageSubscriber> Subscribers { get; set; }
    }
}
