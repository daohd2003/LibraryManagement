﻿using LibraryManagement.Services.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiSearchController : ControllerBase
    {
        private readonly IAiSearchService _aiSearchService;

        public AiSearchController(IAiSearchService aiSearchService)
        {
            _aiSearchService = aiSearchService;
        }

        [HttpGet("ask")]
        public async Task<IActionResult> Ask([FromQuery] string question)
        {
            if (string.IsNullOrEmpty(question))
                return BadRequest("Question is required.");

            var answer = await _aiSearchService.AskAboutLibraryAsync(question);
            return Ok(new { answer });
        }
    }
}
