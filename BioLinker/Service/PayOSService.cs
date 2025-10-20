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

            // ✅ Đảm bảo orderCode là số nguyên dương
            if (dto.OrderCode <= 0)
                dto.OrderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var body = new
            {
                orderCode = dto.OrderCode,
                amount = (int)dto.Amount, // ✅ PayOS chỉ nhận integer
                description = dto.Description,
                cancelUrl = dto.CancelUrl,
                returnUrl = dto.ReturnUrl,
                items = new[]
                {
            new {
                name = dto.ItemName ?? "Goi BioLink Pro",
                quantity = 1,
                price = (int)dto.Amount
            }
        },
                buyerName = "Test User",
                buyerEmail = "test@biolinker.com",
                buyerPhone = "0900000000"
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

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            if (!response.IsSuccessStatusCode)
                throw new Exception($"PayOS error: {raw}");

            if (!root.TryGetProperty("data", out JsonElement data) || data.ValueKind != JsonValueKind.Object)
                throw new Exception($"PayOS error: Response missing 'data' field. Raw: {raw}");

            return new PayOSResponse
            {
                PaymentLink = data.GetProperty("checkoutUrl").GetString() ?? "",
                OrderCode = data.GetProperty("orderCode").ToString(),
                Message = "Tạo liên kết thanh toán thành công"
            };
        
        }

        public bool VerifyChecksum(string payload, string receivedSignature)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(receivedSignature)) return false;

                string key = _cfg["PayOS:ChecksumKey"]!;

                using var doc = JsonDocument.Parse(payload);
                if (!doc.RootElement.TryGetProperty("data", out var dataElement))
                {
                    _logger.LogWarning("Webhook không có trường 'data'.");
                    return false;
                }

                // Build chuỗi kiểu key1=value1&key2=value2… sorted alphabet
                var dict = new SortedDictionary<string, string>(StringComparer.Ordinal);
                foreach (var prop in dataElement.EnumerateObject())
                {
                    // Lấy giá trị string của prop (nếu object/array có thể cần serialize)
                    string value;
                    if (prop.Value.ValueKind == JsonValueKind.Object || prop.Value.ValueKind == JsonValueKind.Array)
                        value = prop.Value.GetRawText();
                    else
                        value = prop.Value.ToString();

                    dict.Add(prop.Name, value);
                }

                string dataString = string.Join("&", dict.Select(kv => $"{kv.Key}={kv.Value}"));

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataString));
                string calc = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                bool match = calc == receivedSignature.ToLower();
                _logger.LogInformation("Signature check: {match} | calc={calc} | recv={recv}", match, calc, receivedSignature);
                return match;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying PayOS signature");
                return false;
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
