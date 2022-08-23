using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MessageBus.Models
{
    public class Subscriber
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        [JsonIgnore]
        public Channel Channel { get; set; }
        public int ChannelId { get; set; }
    }
}
