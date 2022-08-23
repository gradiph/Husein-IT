using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MessageBus.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string PublisherName { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        public int ChannelId { get; set; }
        [JsonIgnore]
        public Channel Channel { get; set; }
    }
}
