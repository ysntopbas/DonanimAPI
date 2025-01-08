namespace DonanimAPI.Models
{
    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string Content { get; set; }
        public DateTime MessageDate { get; set; } = DateTime.UtcNow;
        public bool IsMessageIT { get; set; }
    }
} 