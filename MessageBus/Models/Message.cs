using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MessageBus.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string PublisherName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Data { get; set; }

        [Required]
        public Channel Channel { get; set; }
        public ICollection<MessageSubscriber> Subscribers { get; set; }
    }
}
