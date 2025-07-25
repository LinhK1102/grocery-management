using BusinessObjects.Commons;
using BusinessObjects.DTOs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Services
{
    public class PayOSService
    {
        private readonly HttpClient _httpClient;
        private readonly PayOSSettings _settings;
        public PayOSService(HttpClient httpClient, IOptions<PayOSSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<string> CreatePaymentAsync(CreatePaymentRequest req)
        {
            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            string rawPayload = $"amount={req.amount}&cancelUrl={req.cancelUrl}&description={req.description}&orderCode={orderCode}&returnUrl={req.returnUrl}";

            var apiKey = _settings.ApiKey;
            var clientId = _settings.ClientId;
            var checksumKey = _settings.ChecksumKey;

            var signature = CalculateSignature(rawPayload, checksumKey);

            var payloadObj = new
            {
                amount = req.amount,
                cancelUrl = req.cancelUrl,
                description = req.description,
                orderCode = orderCode,
                returnUrl = req.returnUrl,
                signature = signature
            };

            var json = JsonConvert.SerializeObject(payloadObj, Formatting.None);

            // ✅ Step 4: Send request
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-merchant.payos.vn/v2/payment-requests")
            {
                Headers =
                {
                    { "x-client-id", clientId },
                    { "x-api-key", apiKey }
                },
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        private string CalculateSignature(string payload, string secretKey)
        {
            var encoding = Encoding.UTF8;
            byte[] keyBytes = encoding.GetBytes(secretKey);
            byte[] payloadBytes = encoding.GetBytes(payload);

            using var hmac = new HMACSHA256(keyBytes);
            byte[] hash = hmac.ComputeHash(payloadBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower(); // hex lowercase
        }

    }
}
