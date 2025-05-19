using LibraryManagement.Services.Payments.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    }
}
