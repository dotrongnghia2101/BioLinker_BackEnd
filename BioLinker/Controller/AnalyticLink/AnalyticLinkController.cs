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
        [HttpPost("click/{linkId}")]
        public async Task<IActionResult> RecordClick(string linkId)
        {
            await _analyticService.RecordClickAsync(linkId);
            return Ok(new
            {
                message = $"Click for link {linkId} recorded successfully",
                timestamp = DateTime.UtcNow
            });
        }

        //view cho bio
        [HttpPost("view/{bioPageId}")]
        public async Task<IActionResult> RecordView(string bioPageId)
        {
            await _analyticService.RecordViewAsync(bioPageId);
            return Ok(new
            {
                message = $"View for BioPage {bioPageId} recorded successfully",
                timestamp = DateTime.UtcNow
            });
        }

        //lay toan bo click cua link
        [HttpGet("link/{linkId}")]
        public async Task<IActionResult> GetAnalyticsByLink(string linkId)
        {
            var result = await _analyticService.GetAnalyticsByLinkAsync(linkId);

            var data = result.Select(a => new
            {
                a.Date,
                a.Clicks,
                a.Views
            });

            return Ok(new
            {
                linkId,
                total = data.Sum(x => x.Clicks),
                details = data
            });
        }

        //tong so click cua link 
        [HttpGet("link/{linkId}/total")]
        public async Task<IActionResult> GetTotalClicksByLink(string linkId)
        {
            var total = await _analyticService.GetTotalClicksByLinkAsync(linkId);
            return Ok(new
            {
                linkId,
                totalClicks = total ?? 0
            });
        }

        //tong so click cua link trong bio page
        [HttpGet("page/{bioPageId}/total")]
        public async Task<IActionResult> GetTotalClicksByPage(string bioPageId)
        {
            var total = await _analyticService.GetTotalClicksByPageAsync(bioPageId);
            return Ok(new
            {
                bioPageId,
                totalClicks = total ?? 0
            });
        }
    }
}
