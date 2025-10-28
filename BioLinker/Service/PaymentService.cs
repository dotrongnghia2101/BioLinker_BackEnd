using BioLinker.Data;
using BioLinker.DTO.PaymentDTO;
using BioLinker.Enities;
using BioLinker.Respository.PaymentRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using System.Text.Json;

namespace BioLinker.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;
        private readonly PayOS _payos;
        private readonly ILogger<PaymentService> _logger;
        private readonly AppDBContext _db;
        private readonly string _returnUrl;
        private readonly string _cancelUrl;
        private readonly IEmailSender _emailSender;

        public PaymentService(
             IPaymentRepository repo,
             PayOS payos,
             ILogger<PaymentService> logger,
             AppDBContext db,
             IConfiguration config,
             IEmailSender emailSender)
        {
            _repo = repo;
            _payos = payos;
            _logger = logger;
            _db = db;

            // Doc returnUrl & cancelUrl tu appsettings.json
            _returnUrl = config["PayOSSettings:ReturnUrl"] ?? "https://biolinker.io.vn/account";
            _cancelUrl = config["PayOSSettings:CancelUrl"] ?? "https://biolinker.io.vn/account";
            _emailSender = emailSender;
        }

        public async Task<PayOSResponse> CreatePaymentAsync(PayOSRequest dto)
        {
            try
            {
                long orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var items = new List<ItemData>
                {
                    new ItemData(dto.ItemName ?? "Goi su dung", 1, (int)dto.Amount)
                };

                var paymentData = new PaymentData(
                    orderCode,
                    (int)dto.Amount,
                    dto.Description,
                    items,
                    cancelUrl: _cancelUrl,
                    returnUrl: _returnUrl
                );

                _logger.LogInformation(" Gửi yêu cầu PayOS: {data}", paymentData);

                var res = await _payos.createPaymentLink(paymentData);

                var payment = new Payment
                {
                    PaymentId = Guid.NewGuid().ToString(),
                    UserId = dto.UserId,
                    PlanId = dto.PlanId,
                    OrderCode = orderCode.ToString(),
                    Amount = dto.Amount,
                    Description = dto.Description,
                    Status = "Pending",
                    Method = "PayOS",
                    PaymentUrl = res.checkoutUrl,
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.CreateAsync(payment);

                _logger.LogInformation(" Tạo thanh toán thành công: {orderCode}", orderCode);

                return new PayOSResponse
                {
                    OrderCode = orderCode.ToString(),
                    PaymentLink = res.checkoutUrl,
                    Message = "Tạo liên kết thanh toán thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Lỗi tạo thanh toán PayOS");
                throw;
            }
        }

        public async Task<IEnumerable<PaymentResponse>> GetAllPaymentsAsync()
        {
            try
            {
                var payments = await _repo.GetAllPaymentsAsync();
                _logger.LogInformation("Admin lấy danh sách tất cả thanh toán, tổng cộng {Count}", payments.Count());

                return payments.Select(p => new PaymentResponse
                {
                    PaymentId = p.PaymentId,
                    OrderCode = p.OrderCode,
                    Amount = p.Amount,
                    Status = p.Status,
                    Method = p.Method,
                    PlanId = p.PlanId,
                    UserId = p.UserId,
                    PaymentUrl = p.PaymentUrl,
                    CreatedAt = p.CreatedAt,
                    PaidAt = p.PaidAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi admin lấy danh sách thanh toán");
                throw;
            }
        }

        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByUserAsync(string userId)
        {
            try
            {
                var payments = await _repo.GetPaymentsByUserAsync(userId);
                _logger.LogInformation("Lấy {Count} thanh toán của user {UserId}", payments.Count(), userId);

                return payments.Select(p => new PaymentResponse
                {
                    PaymentId = p.PaymentId,
                    OrderCode = p.OrderCode,
                    Amount = p.Amount,
                    Status = p.Status,
                    Method = p.Method,
                    PlanId = p.PlanId,
                    UserId = p.UserId,
                    PaymentUrl = p.PaymentUrl,
                    CreatedAt = p.CreatedAt,
                    PaidAt = p.PaidAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thanh toán của user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> HandleWebhookAsync(string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(body) || body.Trim() == "{}")
                {
                    _logger.LogInformation("Webhook test (body rỗng hoặc {}) → trả về 200 OK cho PayOS.");
                    return true;
                }

                var webhook = JsonSerializer.Deserialize<WebhookType>(body);
                if (webhook?.data?.orderCode == 123)
                {
                    _logger.LogInformation("Webhook test (orderCode=123) từ PayOS → trả về 200 OK.");
                    return true;
                }

                // ✅ Xác minh chữ ký PayOS
                WebhookData webhookData = _payos.verifyPaymentWebhookData(webhook);

                long orderCode = webhookData.orderCode;
                string code = webhookData.code;
                _logger.LogInformation("📦 Nhận webhook orderCode={OrderCode}, code={Code}", orderCode, code);

                // ✅ Lấy Payment kèm User + Plan
                var payment = await _db.Payments
                    .Include(p => p.User)
                    .ThenInclude(u => u.UserRoles)
                    .Include(p => p.Plan)
                    .FirstOrDefaultAsync(p => p.OrderCode == orderCode.ToString());

                if (payment == null)
                {
                    _logger.LogWarning("Không tìm thấy Payment orderCode={OrderCode}", orderCode);
                    return false;
                }

                // ✅ Xử lý kết quả thanh toán
                if (code == "00")
                {
                    payment.Status = "Paid";
                    payment.PaidAt = DateTime.UtcNow;

                    var user = payment.User;
                    var plan = payment.Plan;

                    if (user != null && plan != null)
                    {
                        // 👉 Tính ngày hết hạn gói
                        DateTime expireAt = plan.DurationUnit == DurationUnit.Month
                            ? payment.PaidAt.Value.AddMonths(plan.Duration ?? 1)
                            : payment.PaidAt.Value.AddYears(plan.Duration ?? 1);

                        user.CurrentPlanId = plan.PlanId;
                        user.PlanExpireAt = expireAt;

                        // 👉 Xác định Role theo gói
                        string roleName = plan.PlanId switch
                        {
                            "PRO-PLAN" => "ProUser",
                            "BUSINESS-PLAN" => "BusinessUser",
                            _ => "FreeUser"
                        };

                        // 👉 Lấy Role tương ứng
                        var role = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
                        if (role != null)
                        {
                            var oldRoles = await _db.UserRoles
                                            .Where(ur => ur.UserId == user.UserId)
                                            .ToListAsync();

                            if (oldRoles.Any())
                            {
                                _db.UserRoles.RemoveRange(oldRoles);
                                _logger.LogInformation("🧹 Đã xóa {Count} role cũ của user {UserId}", oldRoles.Count, user.UserId);
                            }

                            // 🔥 Gán role mới theo gói
                            var newUserRole = new UserRole
                            {
                                UserRoleId = Guid.NewGuid().ToString(),
                                UserId = user.UserId,
                                RoleId = role.RoleId,
                                StartDate = DateTime.UtcNow,
                                EndDate = expireAt
                            };

                            await _db.UserRoles.AddAsync(newUserRole);

                            _logger.LogInformation("✅ Gán role {RoleName} cho user {UserId} (hết hạn: {ExpireAt})",
                                roleName, user.UserId, expireAt);
                        }
                        else
                        {
                            _logger.LogWarning("⚠️ Không tìm thấy role {RoleName}", roleName);
                        }
                        // 👉 Cập nhật và lưu lại tất cả thay đổi
                        _db.Users.Update(user);
                        _db.Payments.Update(payment);
                        await _db.SaveChangesAsync();

                        // 👉 Load lại user.UserRoles để đồng bộ entity
                        await _db.Entry(user).Collection(u => u.UserRoles).LoadAsync();

                        // 📧 Gửi email thông báo thanh toán thành công
                        try
                        {
                            if (!string.IsNullOrEmpty(user.Email))
                            {
                                await _emailSender.SendPaymentSuccessEmailAsync(
                                    user.Email,
                                    plan.PlanName ?? "Pro",
                                    user.PlanExpireAt ?? DateTime.UtcNow.AddMonths(1)
                                );

                                _logger.LogInformation("📧 Đã gửi email xác nhận thanh toán thành công cho {Email}", user.Email);
                            }
                        }
                        catch (Exception mailEx)
                        {
                            _logger.LogError(mailEx, "⚠️ Lỗi khi gửi email thông báo thanh toán cho user {UserId}", user.UserId);
                        }
                    }
                }
                else
                {
                    payment.Status = "Failed";
                    _db.Payments.Update(payment);
                    await _db.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi xử lý webhook PayOS");
                return false;
            }
        }

        public async Task<bool> UpgradeToProPlanAsync(string userId)
        {
            var user = await _db.Users
                       .Include(u => u.UserRoles)
                       .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new Exception("Không tìm thấy người dùng");

            var proPlan = await _db.SubscriptionPlans.FirstOrDefaultAsync(p => p.PlanId == "PRO-PLAN");
            if (proPlan == null)
                throw new Exception("Không tìm thấy gói Pro");

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == "ProUser");
            if (role == null)
                throw new Exception("Không tìm thấy role ProUser");

            // Tính thời gian hết hạn
            DateTime startDate = DateTime.UtcNow;
            DateTime expireAt = startDate.AddMonths(1);

            // Gán plan mới
            user.CurrentPlanId = proPlan.PlanId;
            user.PlanExpireAt = expireAt;

            // Xóa role cũ
            var oldRoles = user.UserRoles.ToList();
            if (oldRoles.Any())
                _db.UserRoles.RemoveRange(oldRoles);

            // Gán role ProUser
            var newRole = new UserRole
            {
                UserRoleId = Guid.NewGuid().ToString(),
                UserId = user.UserId,
                RoleId = role.RoleId,
                StartDate = startDate,
                EndDate = expireAt
            };
            await _db.UserRoles.AddAsync(newRole);

            await _db.SaveChangesAsync();
            // Gửi email thông báo nâng cấp
            await _emailSender.SendUpgradeToProEmailAsync(user.Email, user.FirstName ?? "Người dùng", expireAt);

            _logger.LogInformation("✅ Nâng cấp user {UserId} lên ProUser, hết hạn: {ExpireAt}", userId, expireAt);

            return true;
        }
    }
    }
    
