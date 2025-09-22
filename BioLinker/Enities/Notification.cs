namespace BioLinker.Enities
{
    public class Notification
    {
        public string? NotificationId { get; set; } = Guid.NewGuid().ToString(); // PK
        public string? UserId { get; set; } // FK
        public string? Type { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public virtual User? User { get; set; }
    }
}
