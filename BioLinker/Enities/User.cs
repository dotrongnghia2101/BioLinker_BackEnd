using System.ComponentModel.DataAnnotations;

namespace BioLinker.Enities
{
    public class User
    {
        [Key] public String UserId { get; set; } = Guid.NewGuid().ToString();
        public bool? IsActive { get; set; }
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Description { get; set; }
        public string? NickName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserImage { get; set; }
        public string? PasswordHash { get; set; }
        public string? Job { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? IsGoogle { get; set; }
        public string? CustomerDomain { get; set; } 

        // Navigation
        //public virtual ICollection<SubscriptionPlan> Subscriptions { get; set; } = new List<SubscriptionPlan>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<BioPage> BioPages { get; set; } = new List<BioPage>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<UserTemplate> UserTemplates { get; set; } = new List<UserTemplate>();
        public virtual ICollection<Template> CreatedTemplates { get; set; } = new List<Template>();
        public virtual ICollection<Marketplace> Marketplaces { get; set; } = new List<Marketplace>();
        public virtual ICollection<StaticLink> StaticLinks { get; set; } = new List<StaticLink>();
    }
}
