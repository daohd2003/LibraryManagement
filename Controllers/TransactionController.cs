using Azure.Core;
using LibraryManagement.DTOs.Request;
using LibraryManagement.Services.Payments.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Transactions;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Member")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyTransactions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return BadRequest("UserId không hợp lệ.");

            var transactions = await _transactionService.GetUserTransactionsAsync(userId);

            return Ok(transactions);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return BadRequest("UserId không hợp lệ.");

            // Tạo mã giao dịch duy nhất
            var transactionCode = $"TXN{DateTime.UtcNow.Ticks}";

            var transaction = new Models.Transaction
            {
                UserId = userId,
                Amount = request.Amount,
                PaymentMethod = "SEPAY",
                TransactionCode = transactionCode,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            // Lưu vào DB
            await _transactionService.SaveTransactionAsync(transaction);

            // Tạo link QR Pay (SEPAY)
            string bankCode = "MBBank";
            string acc = "0347350184";
            string template = "compact";
            string des = Uri.EscapeDataString($"{request.Description} - {transactionCode}");

            string qrUrl = $"https://qr.sepay.vn/img?bank={bankCode}&acc={acc}&template={template}&amount={request.Amount}&des={des}";

            return Ok(new
            {
                transaction.Id,
                transaction.TransactionCode,
                transaction.Status,
                transaction.Amount,
                QrImageUrl = qrUrl
            });
        }

        [HttpPost("{transactionId}/pay")]
        public async Task<IActionResult> PayTransaction(int transactionId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không thể xác định người dùng.");

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return BadRequest("UserId không hợp lệ.");

            var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);

            if (transaction == null || transaction.UserId != userId)
                return NotFound("Giao dịch không tồn tại hoặc không thuộc người dùng.");

            if (transaction.Status != "PENDING")
                return BadRequest("Giao dịch đã được thanh toán hoặc không thể thanh toán.");

            // Tạo link QR Pay (SEPAY)
            string bankCode = "MBBank";
            string acc = "0347350184";
            string template = "compact";
            string des = Uri.EscapeDataString($"{"Thanh Toan"} - {transaction.TransactionCode}");

            string qrUrl = $"https://qr.sepay.vn/img?bank={bankCode}&acc={acc}&template={template}&amount={transaction.Amount}&des={des}";

            return Ok(new
            {
                transaction.Id,
                transaction.TransactionCode,
                transaction.Status,
                transaction.Amount,
                QrImageUrl = qrUrl
            });
        }
    }
}
