using System.ComponentModel.DataAnnotations;

namespace MessageBus.Models.DTOs
{
    public class SubscriberDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }

        public Subscriber ToSubscriber()
        {
            Subscriber subscriber = new Subscriber();
            subscriber.Name = Name;
            subscriber.Url = Url;
            return subscriber;
        }
    }
}
