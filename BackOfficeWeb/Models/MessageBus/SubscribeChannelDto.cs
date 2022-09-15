namespace BackOfficeWeb.Models.MessageBus
{
    public class SubscribeChannelDto
    {
        public ICollection<int> ids { get; set; } = new List<int>();
    }
}
