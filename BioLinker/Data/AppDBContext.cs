using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;
using System;

namespace BioLinker.Data
{
    public partial class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<AnalyticLink> AnalyticLinks { get; set; }
        public virtual DbSet<BioPage> BioPages { get; set; }
        public virtual DbSet<Link> Links { get; set; }
        public virtual DbSet<Marketplace> Markets { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<StaticLink> StaticLinks { get; set; }
        public virtual DbSet<Style> Styles { get; set; }
        public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<UserTemplate> UserTemplates { get; set; }
        public virtual DbSet<Content> Contents { get; set; }
        public virtual DbSet<Background> Backgrounds { get; set; }
        public virtual DbSet<StyleSettings> StyleSettings { get; set; }
        public virtual DbSet<TemplateDetail> TemplateDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Fallback connection string 
                optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;database=BioLinkDB;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ========== USER ==========
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId).HasName("PK_Users");
                entity.Property(e => e.UserId)
                    .HasColumnName("userID");
                entity.Property(e => e.Email)
                     .IsRequired()
                     .HasMaxLength(255)
                     .HasColumnName("email");
                entity.Property(e => e.PhoneNumber)
                     .HasMaxLength(20)
                     .HasColumnName("phoneNumber");
                entity.Property(e => e.FirstName)
                      .HasMaxLength(100)
                      .HasColumnName("firstName");
                entity.Property(e => e.LastName)
                     .HasMaxLength(100)
                     .HasColumnName("lastName");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime")
                      .HasColumnName("createdAt");
                entity.Property(e => e.UserImage)
                      .HasMaxLength(500)
                      .HasColumnName("userImage");
                entity.Property(e => e.PasswordHash)
                     .HasMaxLength(255)
                     .HasColumnName("passwordHash");
                entity.Property(e => e.Job)
                      .HasMaxLength(200)
                      .HasColumnName("job");
                entity.Property(e => e.Gender)
                      .HasMaxLength(50)
                      .HasColumnName("gender");
                entity.Property(e => e.IsActive)
                      .HasColumnName("isActive");
                entity.Property(e => e.IsGoogle)
                      .HasColumnName("isGoogle")
                      .HasDefaultValue(false);
                entity.Property(e => e.DateOfBirth)
                      .HasColumnType("date")
                      .HasColumnName("dateOfBirth");
                entity.Property(e => e.CustomerDomain)
                      .HasMaxLength(255)
                      .HasColumnName("customerDomain");
                entity.Property(e => e.Description)
                     .HasColumnName("description");
                entity.Property(e => e.NickName)
                    .HasColumnName("nickName");
            });

            // ========== ROLE ==========
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");
                entity.HasKey(e => e.RoleId).HasName("PK_Role");
                entity.Property(e => e.RoleId).HasColumnName("roleID");
                entity.Property(e => e.RoleName)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnName("roleName");
            });

            // ========== USER ROLE ==========
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");
                entity.HasKey(e => e.UserRoleId).HasName("PK_UserRole");
                entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");
                entity.Property(e => e.UserId).HasColumnName("userID");
                entity.Property(e => e.RoleId).HasColumnName("roleID");
                entity.Property(e => e.StartDate)
                      .HasColumnType("datetime")
                      .HasColumnName("startDate");
                entity.Property(e => e.EndDate)
                      .HasColumnType("datetime")
                      .HasColumnName("endDate");
                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .HasConstraintName("FK_UserRole_User")
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .HasConstraintName("FK_UserRole_Role")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== BIOPAGE ==========
            modelBuilder.Entity<BioPage>(entity =>
            {
                entity.ToTable("BioPage");
                entity.HasKey(e => e.BioPageId)
                      .HasName("PK_BioPage");
                entity.Property(e => e.BioPageId)
                      .HasColumnName("bioPageID");
                entity.Property(e => e.UserId)
                      .HasColumnName("userID");
                entity.Property(e => e.TemplateId)
                      .HasColumnName("templateID");
                entity.Property(e => e.Title)
                      .HasMaxLength(200)
                      .HasColumnName("title");
                entity.Property(e => e.Description)
                      .HasColumnName("description");
                entity.Property(e => e.Avatar)
                      .HasMaxLength(500)
                      .HasColumnName("avatar");
                entity.Property(e => e.Status)
                      .HasMaxLength(50)
                      .HasColumnName("status");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime")
                      .HasColumnName("createdAt");
                entity.Property(e => e.StyleId)
                      .HasColumnName("styleID");
                entity.Property(e => e.BackgroundId)
                      .HasColumnName("backgroundID");
                entity.Property(e => e.StyleSettingsId)
                      .HasColumnName("styleSettingsID");

                entity.HasOne(bp => bp.User)
                      .WithMany(u => u.BioPages)
                      .HasForeignKey(bp => bp.UserId)
                      .HasConstraintName("FK_BioPage_User")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(bp => bp.Template)
                      .WithMany(t => t.BioPages)
                      .HasForeignKey(bp => bp.TemplateId)
                      .HasConstraintName("FK_BioPage_Template")
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(bp => bp.Style)
                      .WithOne(s => s.BioPage)
                      .HasForeignKey<BioPage>(bp => bp.StyleId)
                      .HasConstraintName("FK_BioPage_Style")
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(x => x.Background)
                      .WithMany(b => b.BioPages)
                      .HasForeignKey(x => x.BackgroundId)
                      .HasConstraintName("FK_BioPage_Background")
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(bp => bp.StyleSettings)
                        .WithOne()
                        .HasForeignKey<BioPage>(bp => bp.StyleSettingsId)
                        .HasConstraintName("FK_BioPage_StyleSettings")
                        .OnDelete(DeleteBehavior.SetNull);
            });


            // ========== LINK ==========
            modelBuilder.Entity<Link>(entity =>
            {
                entity.ToTable("Link");
                entity.HasKey(e => e.LinkId)
                      .HasName("PK_Link");
                entity.Property(e => e.LinkId)
                      .HasColumnName("linkID");
                entity.Property(e => e.BioPageId)
                      .HasColumnName("bioPageID");
                entity.Property(e => e.Title)
                      .HasMaxLength(200)
                      .HasColumnName("title");
                entity.Property(e => e.Url)
                      .IsRequired()
                      .HasMaxLength(2000)
                      .HasColumnName("url");
                entity.Property(e => e.Icon)
                      .HasMaxLength(500)
                      .HasColumnName("icon");
                entity.Property(e => e.Position)
                      .HasColumnName("position");
                entity.Property(e => e.ClickCount)
                      .HasColumnName("clickCount");
                entity.Property(e => e.Platform)
                      .HasMaxLength(100)
                      .HasColumnName("platform");
                entity.Property(e => e.LinkType)
                      .HasMaxLength(50)
                      .HasColumnName("linkType");
                entity.Property(e => e.StaticLinkId)
                      .HasColumnName("staticLinkID");

                entity.HasOne(l => l.BioPage)
                      .WithMany(bp => bp.Links)
                      .HasForeignKey(l => l.BioPageId)
                      .HasConstraintName("FK_Link_BioPage")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.StaticLink)
                      .WithMany(sl => sl.Links)
                      .HasForeignKey(l => l.StaticLinkId)
                      .HasConstraintName("FK_Link_StaticLink")
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => new { e.BioPageId, e.Position })
                      .HasDatabaseName("IX_Link_BioPage_Position");
            });

            // ========== STATIC LINK ==========
            modelBuilder.Entity<StaticLink>(e =>
            {
                e.ToTable("StaticLink");
                e.HasKey(x => x.StaticLinkId).HasName("PK_StaticLink");
                e.Property(x => x.StaticLinkId).HasColumnName("staticLinkID");
                e.Property(x => x.Title)
                 .HasColumnType("longtext")
                 .HasColumnName("title");
                e.Property(x => x.Icon)
                 .HasColumnType("longtext")
                 .HasColumnName("icon");
                e.Property(x => x.Platform)
                 .HasMaxLength(100)
                 .HasColumnName("platform");
                e.Property(x => x.DefaultUrl)
                 .HasMaxLength(2000)
                 .HasColumnName("defaultUrl");
            });

            // ========== ANALYTIC LINK ==========
            modelBuilder.Entity<AnalyticLink>(entity =>
            {
                entity.ToTable("AnalyticLink");
                entity.HasKey(e => e.AnalyticsId)
                      .HasName("PK_AnalyticLink");
                entity.Property(e => e.AnalyticsId)
                      .HasColumnName("analyticsID");
                entity.Property(e => e.LinkId)
                      .HasColumnName("linkID");
                entity.Property(e => e.BioPageId)
                      .HasColumnName("bioPageID");
                entity.Property(e => e.Views)
                      .HasColumnName("views");
                entity.Property(e => e.Clicks)
                      .HasColumnName("clicks");
                entity.Property(e => e.Date)
                      .HasColumnType("datetime")
                      .HasColumnName("date");

                entity.HasOne(al => al.Link)
                      .WithMany(l => l.AnalyticLinks)
                      .HasForeignKey(al => al.LinkId)
                      .HasConstraintName("FK_AnalyticLink_Link")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(al => al.BioPage)
                      .WithMany(bp => bp.AnalyticLinks)
                      .HasForeignKey(al => al.BioPageId)
                      .HasConstraintName("FK_AnalyticLink_BioPage")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== CONTENT ==========
            modelBuilder.Entity<Content>(entity =>
            {
                entity.ToTable("Content");
                entity.HasKey(e => e.ContentId)
                      .HasName("PK_Content");

                entity.Property(e => e.ContentId)
                      .HasColumnName("contentID");
                entity.Property(e => e.BioPageId)
                      .HasColumnName("bioPageID");
                entity.Property(e => e.ElementType)
                      .HasMaxLength(50)
                      .HasColumnName("elementType");
                entity.Property(e => e.PositionData)
                      .HasColumnType("longtext");
                entity.Property(e => e.SizeData)
                      .HasColumnType("longtext");
                entity.Property(e => e.ElementData)
                      .HasColumnType("longtext");
                entity.Property(e => e.Alignment)
                      .HasMaxLength(20)
                      .HasColumnName("alignment");
                entity.Property(e => e.Visible)
                      .HasColumnName("visible");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime")
                      .HasColumnName("createdAt");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("datetime")
                      .HasColumnName("updatedAt");

                entity.HasOne(c => c.BioPage)
                      .WithMany(bp => bp.Contents)
                      .HasForeignKey(c => c.BioPageId)
                      .HasConstraintName("FK_Content_BioPage")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== TEMPLATE ==========
            modelBuilder.Entity<Template>(entity =>
            {
                entity.ToTable("Template");
                entity.HasKey(e => e.TemplateId)
                      .HasName("PK_Template");
                entity.Property(e => e.TemplateId)
                      .HasColumnName("templateID");
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(200)
                      .HasColumnName("name");
                entity.Property(e => e.Description)
                      .HasColumnName("description");
                entity.Property(e => e.Category)
                      .HasMaxLength(100)
                      .HasColumnName("category");
                entity.Property(e => e.IsPremium)
                      .HasColumnName("isPremium");
                entity.Property(e => e.CreatedBy)
                      .HasColumnName("createdBy");
                entity.Property(e => e.Status)
                      .HasMaxLength(50)
                      .HasColumnName("status");
                entity.Property(e => e.StyleId)
                      .HasColumnName("styleID");

                entity.Property(e => e.BackgroundId)
                      .HasColumnName("backgroundID");

                entity.Property(e => e.StyleSettingsId)
                      .HasColumnName("styleSettingsID");

                entity.HasOne(t => t.Creator)
                      .WithMany(u => u.CreatedTemplates)
                      .HasForeignKey(t => t.CreatedBy)
                      .HasConstraintName("FK_Template_User")
                      .OnDelete(DeleteBehavior.SetNull);

                // Quan hệ: Template - Style (1-1)
                entity.HasOne(t => t.Style)
                      .WithMany()
                      .HasForeignKey(t => t.StyleId)
                      .HasConstraintName("FK_Template_Style")
                      .OnDelete(DeleteBehavior.SetNull);

                // Quan hệ: Template - Background (1-1)
                entity.HasOne(t => t.Background)
                      .WithMany()
                      .HasForeignKey(t => t.BackgroundId)
                      .HasConstraintName("FK_Template_Background")
                      .OnDelete(DeleteBehavior.SetNull);

                // Quan hệ: Template - StyleSettings (1-1)
                entity.HasOne(t => t.StyleSettings)
                      .WithOne()
                      .HasForeignKey<Template>(t => t.StyleSettingsId)
                      .HasConstraintName("FK_Template_StyleSettings")
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(t => t.TemplateDetails)
                      .WithOne(td => td.Template)
                      .HasForeignKey(td => td.TemplateId)
                      .HasConstraintName("FK_TemplateDetail_Template")
                      .OnDelete(DeleteBehavior.Cascade);

                // Quan hệ: Template - BioPage (1-n)
                entity.HasMany(t => t.BioPages)
                      .WithOne(bp => bp.Template)
                      .HasForeignKey(bp => bp.TemplateId)
                      .HasConstraintName("FK_BioPage_Template")
                      .OnDelete(DeleteBehavior.SetNull);

                // Quan hệ: Template - Marketplace (1-n)
                entity.HasMany(t => t.Marketplaces)
                      .WithOne(m => m.Template)
                      .HasForeignKey(m => m.TemplateId)
                      .HasConstraintName("FK_Marketplace_Template")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== TEMPLATE DETAIL ==========
            modelBuilder.Entity<TemplateDetail>(entity =>
            {
                entity.ToTable("TemplateDetail");

                entity.HasKey(e => e.TemplateDetailId)
                      .HasName("PK_TemplateDetail");

                entity.Property(e => e.TemplateDetailId)
                      .HasColumnName("templateDetailID");

                entity.Property(e => e.TemplateId)
                      .HasColumnName("templateID");

                entity.Property(e => e.ElementType)
                      .HasMaxLength(100)
                      .HasColumnName("elementType");

                entity.Property(e => e.PositionData)
                      .HasColumnName("positionData");

                entity.Property(e => e.SizeData)
                      .HasColumnName("sizeData");

                entity.Property(e => e.StyleData)
                      .HasColumnName("styleData");

                entity.Property(e => e.ElementData)
                      .HasColumnName("elementData");

                entity.Property(e => e.OrderIndex)
                      .HasColumnName("orderIndex");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("createdAt");

                entity.HasOne(td => td.Template)
                      .WithMany(t => t.TemplateDetails)
                      .HasForeignKey(td => td.TemplateId)
                      .HasConstraintName("FK_TemplateDetail_Template")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== USER TEMPLATE ==========
            modelBuilder.Entity<UserTemplate>(entity =>
            {
                entity.ToTable("UserTemplate");
                entity.HasKey(e => e.UTemplateId)
                      .HasName("PK_UserTemplate");
                entity.Property(e => e.UTemplateId)
                      .HasColumnName("uTemplateID");
                entity.Property(e => e.UserId)
                      .HasColumnName("userID");
                entity.Property(e => e.TemplateId)
                      .HasColumnName("templateID");
                entity.Property(e => e.PurchaseAt)
                      .HasColumnType("datetime")
                      .HasColumnName("purchaseAt");
                entity.Property(e => e.PricePaid)
                      .HasColumnType("decimal(18,2)")
                      .HasColumnName("pricePaid");
                entity.Property(e => e.Currency)
                      .HasMaxLength(10)
                      .HasColumnName("currency");
                entity.Property(e => e.ExpireDate)
                      .HasColumnType("datetime")
                      .HasColumnName("expireDate");

                entity.HasOne(ut => ut.User)
                      .WithMany(u => u.UserTemplates)
                      .HasForeignKey(ut => ut.UserId)
                      .HasConstraintName("FK_UserTemplate_User")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ut => ut.Template)
                      .WithMany(t => t.UserTemplates)
                      .HasForeignKey(ut => ut.TemplateId)
                      .HasConstraintName("FK_UserTemplate_Template")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== MARKETPLACE ==========
            modelBuilder.Entity<Marketplace>(entity =>
            {
                entity.ToTable("Marketplace");
                entity.HasKey(e => e.MarketplaceId)
                      .HasName("PK_Marketplace");
                entity.Property(e => e.MarketplaceId)
                      .HasColumnName("marketplaceID");
                entity.Property(e => e.TemplateId)
                      .HasColumnName("templateID");
                entity.Property(e => e.SellerId)
                      .HasColumnName("sellerID");
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)")
                      .HasColumnName("price");
                entity.Property(e => e.SalesCount)
                      .HasColumnName("salesCount");

                entity.HasOne(m => m.Template)
                      .WithMany(t => t.Marketplaces)
                      .HasForeignKey(m => m.TemplateId)
                      .HasConstraintName("FK_Marketplace_Template")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Seller)
                      .WithMany(u => u.Marketplaces)
                      .HasForeignKey(m => m.SellerId)
                      .HasConstraintName("FK_Marketplace_User")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== SUBSCRIPTION PLAN ==========
            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.ToTable("SubscriptionPlan");
                entity.HasKey(e => e.PlanId)
                      .HasName("PK_SubscriptionPlan");
                entity.Property(e => e.PlanId)
                      .HasColumnName("planID");
                entity.Property(e => e.PlanName)
                      .HasMaxLength(200)
                      .HasColumnName("planName");
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)")
                      .HasColumnName("price");
                entity.Property(e => e.Duration)
                      .HasMaxLength(50)
                      .HasColumnName("duration");
            });

            // ========== PAYMENT ==========
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");
                entity.HasKey(e => e.PaymentId)
                      .HasName("PK_Payment");
                entity.Property(e => e.PaymentId)
                      .HasColumnName("paymentID");
                entity.Property(e => e.UserId)
                      .HasColumnName("userID");
                entity.Property(e => e.PlanId)
                      .HasColumnName("planID");
                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)")
                      .HasColumnName("amount");
                entity.Property(e => e.Method)
                      .HasMaxLength(100)
                      .HasColumnName("method");
                entity.Property(e => e.Status)
                      .HasMaxLength(50)
                      .HasColumnName("status");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime")
                      .HasColumnName("createdAt");

                entity.HasOne(p => p.User)
                      .WithMany(u => u.Payments)
                      .HasForeignKey(p => p.UserId)
                      .HasConstraintName("FK_Payment_User")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Plan)
                      .WithMany(sp => sp.Payments)
                      .HasForeignKey(p => p.PlanId)
                      .HasConstraintName("FK_Payment_Plan")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== NOTIFICATION ==========
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");
                entity.HasKey(e => e.NotificationId)
                      .HasName("PK_Notification");
                entity.Property(e => e.NotificationId)
                      .HasColumnName("notificationID");
                entity.Property(e => e.UserId)
                      .HasColumnName("userID");
                entity.Property(e => e.Type)
                      .HasMaxLength(100)
                      .HasColumnName("type");
                entity.Property(e => e.Title)
                      .HasMaxLength(200)
                      .HasColumnName("title");
                entity.Property(e => e.Description)
                      .HasColumnName("description");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime")
                      .HasColumnName("createdAt");

                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .HasConstraintName("FK_Notification_User")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ================= STYLE =================
            modelBuilder.Entity<Style>(entity =>
            {
                entity.ToTable("Style");
                entity.HasKey(e => e.StyleId)
                      .HasName("PK_Style");

                entity.Property(e => e.StyleId)
                      .HasColumnName("styleID");
                entity.Property(e => e.Preset)
                      .HasMaxLength(100)
                      .HasColumnName("preset");
                entity.Property(e => e.LayoutMode)
                      .HasMaxLength(50)                
                      .HasColumnName("layoutMode");
                entity.Property(e => e.ButtonShape)
                      .HasMaxLength(50)
                      .HasColumnName("ButtonShape");
                entity.Property(e => e.ButtonColor)
                      .HasMaxLength(20)
                      .HasColumnName("buttonColor");
                entity.Property(e => e.IconColor)
                      .HasMaxLength(20)
                      .HasColumnName("iconColor");
                entity.Property(e => e.BackgroundColor)
                      .HasMaxLength(20)
                      .HasColumnName("backgroundColor");

            });

            // ========== BACKGROUND ==========
            modelBuilder.Entity<Background>(entity =>
            {
                entity.ToTable("Background");
                entity.HasKey(e => e.BackgroundId)
                      .HasName("PK_Background");

                entity.Property(e => e.BackgroundId)
                      .HasColumnName("backgroundID");
                entity.Property(e => e.Type)
                      .HasMaxLength(50)
                      .HasColumnName("type");
                entity.Property(e => e.Value)
                      .HasMaxLength(500)
                      .HasColumnName("value");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime")
                      .HasColumnName("createdAt");

                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("datetime")
                      .HasColumnName("updatedAt");
            });

            // ========== STYLE SETTINGS ==========
            modelBuilder.Entity<StyleSettings>(entity =>
            {
                entity.ToTable("StyleSettings");
                entity.HasKey(e => e.StyleSettingsId)
                      .HasName("PK_StyleSettings");

                entity.Property(e => e.StyleSettingsId)
                      .HasColumnName("styleSettingsID");
                entity.Property(e => e.Thumbnail)
                      .HasMaxLength(500)
                      .HasColumnName("thumbnail");
                entity.Property(e => e.MetaTitle)
                      .HasMaxLength(200)
                      .HasColumnName("metaTitle");
                entity.Property(e => e.MetaDescription)
                      .HasMaxLength(500)
                      .HasColumnName("metaDescription");
                entity.Property(e => e.CookieBanner)
                      .HasColumnName("cookieBanner");

            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    
    }
}
