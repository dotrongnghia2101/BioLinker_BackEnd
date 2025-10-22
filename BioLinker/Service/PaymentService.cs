using BioLinker.Data;
using BioLinker.DTO.PaymentDTO;
using BioLinker.Enities;
using BioLinker.Respository.PaymentRepo;
using Microsoft.EntityFrameworkCore;
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

        public PaymentService(
             IPaymentRepository repo,
             PayOS payos,
             ILogger<PaymentService> logger,
             AppDBContext db,
             IConfiguration config)
        {
            _repo = repo;
            _payos = payos;
            _logger = logger;
            _db = db;

            // Doc returnUrl & cancelUrl tu appsettings.json
            _returnUrl = config["PayOSSettings:ReturnUrl"] ?? "https://biolinker.io.vn/account";
            _cancelUrl = config["PayOSSettings:CancelUrl"] ?? "https://biolinker.io.vn/account";
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

                // Xác minh chữ ký và lấy data
                WebhookData webhookData = _payos.verifyPaymentWebhookData(webhook);

                long orderCode = webhookData.orderCode;
                int amount = webhookData.amount;                    // nếu cần
                string code = webhookData.code;                     // "00" = thành công
                _logger.LogInformation("📦 Nhận webhook orderCode={OrderCode}, code={Code}", orderCode, code);

                // Tìm payment trong DB theo orderCode
                var payment = await _repo.GetByOrderCodeAsync(orderCode.ToString());
                if (payment == null)
                {
                    _logger.LogWarning("Không tìm thấy đơn hàng orderCode={OrderCode}", orderCode);
                    return false;
                }

                // Cập nhật trạng thái
                if (code == "00") // Thanh toan thanh cong
                {
                    payment.Status = "Paid";
                    payment.PaidAt = DateTime.UtcNow;

                    var user = payment.User;
                    var plan = payment.Plan;

                    if (user != null && plan != null)
                    {
                        // Tinh ngay het han
                        DateTime expireAt = plan.DurationUnit == DurationUnit.Month
                            ? payment.PaidAt.Value.AddMonths(plan.Duration ?? 1)
                            : payment.PaidAt.Value.AddYears(plan.Duration ?? 1);

                        user.CurrentPlanId = plan.PlanId;
                        user.PlanExpireAt = expireAt;

                        // Map SubscriptionPlan -> RoleName
                        string roleName = plan.PlanId switch
                        {
                            "PRO-PLAN" => "ProUser",
                            "BUSINESS-PLAN" => "BusinessUser",
                            _ => "FreeUser"
                        };

                        // Tim Role tu DB
                        var role = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
                        if (role != null)
                        {
                            user.UserRoles.Clear();
                            user.UserRoles.Add(new UserRole
                            {
                                UserId = user.UserId,
                                RoleId = role.RoleId
                            });

                            _logger.LogInformation("✅ Gan role {RoleName} cho user {UserId}, han den {ExpireAt}",
                                roleName, user.UserId, expireAt);
                        }
                        else
                        {
                            _logger.LogWarning("⚠️ Khong tim thay role tuong ung {RoleName}", roleName);
                        }
                    }
                }
                else
                {
                    payment.Status = "Failed";
                }

                await _repo.UpdateAsync(payment);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Loi xu ly webhook PayOS");
                return false;
            }
        }
    }
}