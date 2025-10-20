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

        public PayOSService(IConfiguration cfg)
        {
            _cfg = cfg;
            _http = new HttpClient();
        }
        public async Task<PayOSResponse> CreatePaymentAsync(PayOSRequest dto)
        {
            string clientId = _cfg["PayOS:ClientId"]!;
            string apiKey = _cfg["PayOS:ApiKey"]!;
            string baseUrl = _cfg["PayOS:BaseUrl"]!;

            var body = new
            {
                orderCode = dto.OrderCode,
                amount = dto.Amount,
                description = dto.Description,
                returnUrl = dto.ReturnUrl,
                cancelUrl = dto.CancelUrl
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("x-client-id", clientId);
            _http.DefaultRequestHeaders.Add("x-api-key", apiKey);

            var response = await _http.PostAsync($"{baseUrl}/payments", content);
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"PayOS error: {raw}");

            using var doc = JsonDocument.Parse(raw);
            var data = doc.RootElement.GetProperty("data");
            string link = data.GetProperty("checkoutUrl").GetString() ?? "";
            string oc = data.GetProperty("orderCode").GetString() ?? dto.OrderCode;

            return new PayOSResponse
            {
                PaymentLink = link,
                OrderCode = oc,
                Message = "Thanh toan thanh cong"
            };
        }

        public  bool VerifyChecksum(string payload, string receivedChecksum)
        {
            string key = _cfg["PayOS:ChecksumKey"]!;
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            string calc = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return calc == receivedChecksum.ToLower();
        }
    }
}
