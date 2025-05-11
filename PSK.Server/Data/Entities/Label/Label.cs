using System.Text.Json.Serialization;

namespace PSK.Server.Data.Entities
{
    public class Label
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string TextColor { get; set; }
        public string BackgroundColor { get; set; }


        public Guid TeamId { get; set; }
        public Team Team { get; set; }
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
