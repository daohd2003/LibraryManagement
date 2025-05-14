using LibraryManagement.DTOs.Request;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PenaltyController : ControllerBase
    {
        private readonly IPenaltyService _penaltyService;

        public PenaltyController(IPenaltyService penaltyService)
        {
            _penaltyService = penaltyService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePenalty([FromBody] PenaltyRequestDto request)
        {
            try
            {
                var penalty = await _penaltyService.CreatePenaltyAsync(request.BorrowedBookId, request.ViolationType);
                return Ok(penalty);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
