using LibraryManagement.DTOs.Request;
using LibraryManagement.Services.Payments.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/sepay")]
    [ApiController]
    public class SepayWebhookController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<SepayWebhookController> _logger;

        public SepayWebhookController(ITransactionService transactionService, ILogger<SepayWebhookController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] SepayWebhookRequest request)
        {
            var result = await _transactionService.ProcessSepayWebhookAsync(request);

            if (!result)
            {
                _logger.LogWarning("Webhook processing failed for: {TransactionCode}", request.TransactionCode);
                return BadRequest("Failed to process webhook");
            }

            return Ok(new { message = "Transaction updated" });
        }
    }
}
