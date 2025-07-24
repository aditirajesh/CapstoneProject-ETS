using System.Security.Claims;
using ExpenseTrackingSystem.Interfaces;
using ExpenseTrackingSystem.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestionsController : ControllerBase
    {
        private readonly ISuggestionService _suggestionService;
        private readonly ILogger<SuggestionsController> _logger;

        public SuggestionsController(ISuggestionService suggestionService, ILogger<SuggestionsController> logger)
        {
            _suggestionService = suggestionService;
            _logger = logger;
        }

        private string GetCurrentUsername()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? throw new UnauthorizedAccessException("Username not found in token.");
        }

        /// <summary>
        /// Creates a new suggestion for a user. Only accessible by Analysers.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Analyser")]
        public async Task<IActionResult> CreateSuggestion([FromBody] SuggestionAddDto suggestionDto)
        {
            var analyserUsername = GetCurrentUsername();
            _logger.LogInformation("Analyser {AnalyserUsername} is creating a suggestion for user {TargetUsername}", analyserUsername, suggestionDto.TargetUsername);

            try
            {
                var createdSuggestion = await _suggestionService.CreateSuggestionAsync(analyserUsername, suggestionDto);
                // The service should also trigger the email notification here.
                return CreatedAtAction(nameof(GetSuggestionById), new { id = createdSuggestion.Id }, createdSuggestion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating suggestion for user {TargetUsername}", suggestionDto.TargetUsername);
                return StatusCode(500, new { message = "An error occurred while creating the suggestion." });
            }
        }

        /// <summary>
        /// Gets all suggestions received by the currently logged-in user.
        /// </summary>
        [HttpGet("received")]
        public async Task<ActionResult<IEnumerable<SuggestionDto>>> GetReceivedSuggestions()
        {
            var targetUsername = GetCurrentUsername();
            _logger.LogInformation("User {TargetUsername} is fetching their received suggestions.", targetUsername);

            var suggestions = await _suggestionService.GetSuggestionsForUserAsync(targetUsername);
            return Ok(suggestions);
        }

        /// <summary>
        /// Marks a specific suggestion as read for the logged-in user.
        /// </summary>
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkSuggestionAsRead(int id)
        {
            var username = GetCurrentUsername();
            _logger.LogInformation("User {Username} is marking suggestion {Id} as read.", username, id);

            try
            {
                await _suggestionService.MarkSuggestionAsReadAsync(id, username);
                return NoContent(); // Success, no content to return
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt by user {Username} to mark suggestion {Id} as read.", username, id);
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Suggestion {Id} not found for user {Username} to mark as read.", id, username);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking suggestion {Id} as read for user {Username}", id, username);
                return StatusCode(500, new { message = "An error occurred." });
            }
        }
        
        // Helper endpoint to retrieve a suggestion by ID, used by CreatedAtAction
        [HttpGet("{id}")]
        public async Task<ActionResult<SuggestionDto>> GetSuggestionById(int id)
        {
            var suggestion = await _suggestionService.GetSuggestionByIdAsync(id);
            if (suggestion == null) return NotFound();
            
            // Add authorization check to ensure only related parties can view it
            var currentUser = GetCurrentUsername();
            if (currentUser != suggestion.AnalyserUsername && currentUser != suggestion.TargetUsername)
            {
                return Forbid();
            }

            return Ok(suggestion);
        }
    }
}
