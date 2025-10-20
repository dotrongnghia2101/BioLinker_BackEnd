using BioLinker.DTO.PaymentDTO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BioLinker.Service
{
    public class PayOSService : IPayOSService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _cfg;
        private readonly ILogger<PayOSService> _logger;

        public PayOSService(IConfiguration cfg, ILogger<PayOSService> logger)
        {
            _cfg = cfg;
            _logger = logger;
            _http = new HttpClient();
        }
        public async Task<PayOSResponse> CreatePaymentAsync(PayOSRequest dto)
        {
            string clientId = _cfg["PayOS:ClientId"]!;
            string apiKey = _cfg["PayOS:ApiKey"]!;
            string baseUrl = _cfg["PayOS:BaseUrl"]!;
            string checksumKey = _cfg["PayOS:ChecksumKey"]!;

            var body = new
            {
                orderCode = dto.OrderCode,
                amount = dto.Amount,
                description = dto.Description,
                returnUrl = dto.ReturnUrl,
                cancelUrl = dto.CancelUrl,
                items = new[]
                {
                    new {
                        name = dto.ItemName,
                        quantity = 1,
                        price = dto.Amount
                    }
                }
            };

            string rawData = JsonSerializer.Serialize(body);
            string signature = GenerateHmacSignature(rawData, checksumKey);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v2/payment-requests")
            {
                Content = new StringContent(rawData, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-client-id", clientId);
            request.Headers.Add("x-api-key", apiKey);
            request.Headers.Add("x-checksum", signature);

            _logger.LogInformation("➡️ Gửi yêu cầu PayOS: {payload}", rawData);

            var response = await _http.SendAsync(request);
            var raw = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("⬅️ Phản hồi PayOS: {raw}", raw);

            //  Kiểm tra lỗi HTTP
            if (!response.IsSuccessStatusCode)
                throw new Exception($"PayOS error: {raw}");

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            if (!root.TryGetProperty("data", out JsonElement data) || data.ValueKind != JsonValueKind.Object)
                throw new Exception($"PayOS error: Response missing 'data' field. Raw: {raw}");

            string link = data.GetProperty("checkoutUrl").GetString() ?? "";
            string oc = data.GetProperty("orderCode").GetRawText();

            return new PayOSResponse
            {
                PaymentLink = link,
                OrderCode = oc,
                Message = "Tạo liên kết thanh toán thành công"
            };
        }

        public bool VerifyChecksum(string payload, string receivedChecksum)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(receivedChecksum))
                    return false;

                string key = _cfg["PayOS:ChecksumKey"]!;

                // ✅ Lấy phần "data" để hash
                using var doc = JsonDocument.Parse(payload);
                if (!doc.RootElement.TryGetProperty("data", out var dataElement))
                {
                    _logger.LogWarning("Webhook không có trường 'data' → bỏ qua verify.");
                    return false;
                }

                // ✅ Chuyển phần data sang JSON chuẩn hóa (minified + sorted keys)
                string normalizedData = NormalizeJson(dataElement);

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(normalizedData));
                string calc = BitConverter.ToString(hash).Replace("-", "").ToLower();

                bool match = calc == receivedChecksum.ToLower();
                _logger.LogInformation("Xác minh chữ ký PayOS: {match} | calc={calc} | recv={receivedChecksum}", match, calc, receivedChecksum);

                return match;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác minh checksum");
                return false;
            }
        }

        private string NormalizeJson(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                var props = element.EnumerateObject()
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .Select(p => $"\"{p.Name}\":{NormalizeJson(p.Value)}");
                return "{" + string.Join(",", props) + "}";
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                var items = element.EnumerateArray().Select(NormalizeJson);
                return "[" + string.Join(",", items) + "]";
            }
            else
            {
                return JsonSerializer.Serialize(element.GetRawText().Trim('"')).Trim('"');
            }
        }

        private string GenerateHmacSignature(string rawData, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
