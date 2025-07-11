using System.Text.Json;
using WebApplication.Models;
using Microsoft.AspNetCore.Http;
using WebApplication.Service;
using WebApplication.Constants;
using WebApplication.Helpers;
using ProductDto = WebApplication.Models.Dto.ProductDto;
using BusinessObjects.DTOs;
using Utility.Common;
using Utility.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace WebApplication.Services
{
    public class BarcodeApiService : ApiClientService
    {
        public BarcodeApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config, IHubContext<NotificationHub> hubContext)
            : base(factory, accessor, config, hubContext) { }

        public async Task<UpcProductResponse?> SearchProductAsync(string barcode)
        {
            var client = CreateClient();
            var url = ApiRoutes.Barcode.Search.Replace("{0}", barcode);
            var res = await client.GetAsync(url);
            return await JsonUtility.DeserializeApiResponseAsync<UpcProductResponse>(res).ContinueWith(t => t.Result.Data);
        }

        public async Task<ProductDto?> ScanOrCreateProductAsync(string barcode)
        {
            var client = CreateClient();
            var url = ApiRoutes.Barcode.Scan.Replace("{0}", barcode);
            var res = await client.GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<ProductDto>(res);
            return apiResponse.Data;
        }
    }


    public class ScanProductResponse
    {
        public string Status { get; set; } = "";
        public ProductDto? Lists { get; set; }
    }
}
