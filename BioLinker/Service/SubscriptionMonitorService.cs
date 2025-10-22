using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Service
{
    // ✅ Lop nay chay nen tu dong de kiem tra han su dung cua user
    public class SubscriptionMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubscriptionMonitorService> _logger;

        public SubscriptionMonitorService(IServiceProvider serviceProvider, ILogger<SubscriptionMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // ✅ override chinh xac ham ExecuteAsync cua BackgroundService
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("💡 SubscriptionMonitorService bat dau chay...");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                    var now = DateTime.UtcNow;

                    var expiredUsers = await db.Users
                        .Include(u => u.UserRoles)
                        .Where(u => u.PlanExpireAt < now && u.CurrentPlanId != "FREE-PLAN")
                        .ToListAsync(stoppingToken);

                    foreach (var user in expiredUsers)
                    {
                        var freeRole = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == "FreeUser");

                        user.CurrentPlanId = "FREE-PLAN";
                        user.PlanExpireAt = null;
                        user.UserRoles.Clear();

                        if (freeRole != null)
                        {
                            user.UserRoles.Add(new UserRole
                            {
                                UserId = user.UserId,
                                RoleId = freeRole.RoleId
                            });
                        }

                        _logger.LogInformation("⏳ User {UserId} het han goi, chuyen ve FreeUser", user.UserId);
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }

                // ✅ Doi 24 gio roi chay lai
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}