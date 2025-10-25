using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.AnalyticLink
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticLinkController : ControllerBase
    {
        private readonly IAnalyticLinkService _analyticService;

        public AnalyticLinkController(IAnalyticLinkService analyticService)
        {
            _analyticService = analyticService;
        }

        //click cua link
        [HttpPost("click/{staticLinkId}")]
        public async Task<IActionResult> RecordClick(string staticLinkId)
        {
            await _analyticService.RecordClickAsync(staticLinkId);
            return Ok(new { message = "Click recorded successfully", staticLinkId });
        }

        [HttpGet("{staticLinkId}/click-details")]
        public async Task<IActionResult> GetClickDetails(string staticLinkId)
        {
            var data = await _analyticService.GetClickDetailsAsync(staticLinkId);
            return Ok(data);
        }

        // Lay lich su click cua tat ca link thuoc 1 user
        [HttpGet("user/{userId}/click-history")]
        public async Task<IActionResult> GetUserClickHistory(string userId)
        {
            var data = await _analyticService.GetClickDetailsByUserAsync(userId);
            return Ok(data);
        }

        // Lay tat ca click (debug)
        [HttpGet("clicks/all")]
        public async Task<IActionResult> GetAllClick()
        {
            var data = await _analyticService.GetAllClickAsync();
            return Ok(data);
        }

        //Ghi nhận 1 lượt view cho StaticLink
        [HttpPost("view/{staticLinkId}")]
        public async Task<IActionResult> RecordView(string staticLinkId)
        {
            await _analyticService.RecordViewAsync(staticLinkId);
            return Ok(new { message = "View recorded successfully", staticLinkId });
        }

        // Lấy analytic chi tiết theo StaticLink (danh sách theo ngày)
        [HttpGet("{staticLinkId}/details")]
        public async Task<IActionResult> GetAnalyticsByStaticLink(string staticLinkId)
        {
            var data = await _analyticService.GetAnalyticsByStaticLinkAsync(staticLinkId);
            return Ok(data);
        }

        [HttpGet("{staticLinkId}/summary")]
        public async Task<IActionResult> GetStaticLinkSummary(string staticLinkId)
        {
            var clicks = await _analyticService.GetTotalClicksByStaticLinkAsync(staticLinkId) ?? 0;
            var views = await _analyticService.GetTotalViewsByStaticLinkAsync(staticLinkId) ?? 0;

            double ctr = views > 0 ? Math.Round((double)clicks / views * 100, 2) : 0;

            return Ok(new
            {
                staticLinkId,
                totalViews = views,
                totalClicks = clicks,
                CTR = $"{ctr} %"
            });
        }

       
    }
}
