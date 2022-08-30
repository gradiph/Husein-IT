using System.ComponentModel.DataAnnotations;

namespace MessageBus.Models.DTOs
{
    public class ChannelDto
    {
        [Required]
        public string Name { get; set; }
    }
}
