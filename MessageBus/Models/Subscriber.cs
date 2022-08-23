using System.ComponentModel.DataAnnotations.Schema;

namespace MessageBus.Models
{
    public class Subscriber
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string Url { get; set; }

        public virtual Channel Channel { get; set; }
    }
}
