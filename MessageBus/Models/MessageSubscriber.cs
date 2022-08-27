using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MessageBus.Models
{
    public class MessageSubscriber
    {
        [Key, Column(Order = 0)]
        public int MessageId { get; set; }
        [Key, Column(Order = 1)]
        public int SubscriberId { get; set; }
        public Message Message { get; set; }
        public Subscriber Subscriber { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
