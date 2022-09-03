using System.ComponentModel.DataAnnotations;

namespace MessageBus.Models.DTOs
{
    public class SubscribeChannelDto
    {
        [Required]
        public int[] Ids { get; set; }
    }
}
