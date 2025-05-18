using LibraryManagement.Data;
using LibraryManagement.DTOs.Request;
using LibraryManagement.Services.Documents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController : ControllerBase
    {
        private readonly ReceiptService _receiptService;

        public ReceiptController(ReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> GenerateBorrowReceipt([FromBody] List<ReceiptRequest> requests)
        {
            try
            {
                var filePaths = new List<string>();

                var groupedRequests = requests
                    .GroupBy(r => new { r.UserId, Date = r.Date.Date })
                    .Select(g => new { g.Key.UserId, g.Key.Date });

                foreach (var req in groupedRequests)
                {
                    var filePath = await _receiptService.GenerateBorrowReceiptPdf(req.UserId, req.Date);
                    filePaths.Add(Path.GetFileName(filePath));
                }

                return Ok(new { files = filePaths });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("return")]
        public async Task<IActionResult> GenerateReturnReceipt([FromBody] List<ReceiptRequest> requests)
        {
            try
            {
                var filePaths = new List<string>();

                var groupedRequests = requests
                    .GroupBy(r => new { r.UserId, Date = r.Date.Date })
                    .Select(g => new { g.Key.UserId, g.Key.Date });

                foreach (var req in groupedRequests)
                {
                    var filePath = await _receiptService.GenerateReturnReceiptPdf(req.UserId, req.Date);
                    filePaths.Add(Path.GetFileName(filePath));
                }

                return Ok(new { files = filePaths });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
