using Google.Apis.Drive.v3;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Repositories.Services;

namespace GroceryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriveController : ControllerBase
    {
        private readonly GoogleDriveService _driveService;

        public DriveController(GoogleDriveService driveService)
        {
            _driveService = driveService;
        }

        [HttpGet("quota")]
        public async Task<IActionResult> GetStorageQuota()
        {
            try
            {
                var quota = await _driveService.GetStorageQuotaAsync();
                return Ok(new
                {
                    success = true,
                    message = "Drive quota fetched",
                    data = quota
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to fetch Drive quota",
                    error = ex.Message
                });
            }
        }


    }
}
