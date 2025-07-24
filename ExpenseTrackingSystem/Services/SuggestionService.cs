using ExpenseTrackingSystem.Contexts;
using ExpenseTrackingSystem.Interfaces;
using ExpenseTrackingSystem.Models;
using ExpenseTrackingSystem.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackingSystem.Services
{
    public class SuggestionService : ISuggestionService
    {
        private readonly ExpenseContext _context;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ILogger<SuggestionService> _logger;

        public SuggestionService(
            ExpenseContext context, 
            IUserService userService, 
            IEmailService emailService, 
            ILogger<SuggestionService> logger)
        {
            _context = context;
            _userService = userService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<SuggestionDto> CreateSuggestionAsync(string analyserUsername, SuggestionAddDto suggestionDto)
        {
            // 1. Validate the target user exists
            var targetUser = await _userService.GetUserByUsername(suggestionDto.TargetUsername);
            if (targetUser == null)
            {
                throw new KeyNotFoundException($"User '{suggestionDto.TargetUsername}' not found.");
            }

            // 2. Create the entity
            var suggestion = new Suggestion
            {
                AnalyserUsername = analyserUsername,
                TargetUsername = suggestionDto.TargetUsername,
                Content = suggestionDto.Content,
                ReportPeriodStart = suggestionDto.ReportPeriodStart,
                ReportPeriodEnd = suggestionDto.ReportPeriodEnd,
                CreatedAt = DateTimeOffset.UtcNow,
                IsRead = false
            };

            // 3. Save to database
            await _context.Suggestions.AddAsync(suggestion);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Suggestion {Id} created by {Analyser} for {TargetUser}", suggestion.Id, analyserUsername, suggestionDto.TargetUsername);

            // 4. Send email notification
            try
            {
                string reportPeriod = $"{suggestion.ReportPeriodStart:d MMMM yyyy} to {suggestion.ReportPeriodEnd:d MMMM yyyy}";
                await _emailService.SendSuggestionEmailAsync(targetUser.Username, analyserUsername, suggestion.Content, reportPeriod);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Suggestion {Id} was saved, but failed to send email notification to {Email}", suggestion.Id, targetUser.Username);
                // We don't re-throw the error, because the suggestion was successfully created.
            }

            // 5. Return the created DTO
            return new SuggestionDto
            {
                Id = suggestion.Id,
                AnalyserUsername = suggestion.AnalyserUsername,
                TargetUsername = suggestion.TargetUsername,
                Content = suggestion.Content,
                ReportPeriodStart = suggestion.ReportPeriodStart,
                ReportPeriodEnd = suggestion.ReportPeriodEnd,
                IsRead = suggestion.IsRead,
                CreatedAt = suggestion.CreatedAt
            };
        }

        public async Task<IEnumerable<SuggestionDto>> GetSuggestionsForUserAsync(string targetUsername)
        {
            return await _context.Suggestions
                .Where(s => s.TargetUsername == targetUsername)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new SuggestionDto
                {
                    Id = s.Id,
                    AnalyserUsername = s.AnalyserUsername,
                    TargetUsername = s.TargetUsername,
                    Content = s.Content,
                    ReportPeriodStart = s.ReportPeriodStart,
                    ReportPeriodEnd = s.ReportPeriodEnd,
                    IsRead = s.IsRead,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();
        }

        public async Task MarkSuggestionAsReadAsync(int suggestionId, string username)
        {
            var suggestion = await _context.Suggestions.FindAsync(suggestionId);

            if (suggestion == null)
            {
                throw new KeyNotFoundException("Suggestion not found.");
            }

            // Security check: only the target user can mark it as read
            if (suggestion.TargetUsername != username)
            {
                throw new UnauthorizedAccessException("You are not authorized to modify this suggestion.");
            }

            suggestion.IsRead = true;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Suggestion {Id} marked as read by user {Username}", suggestionId, username);
        }

        public async Task<SuggestionDto?> GetSuggestionByIdAsync(int suggestionId)
        {
            var suggestion = await _context.Suggestions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == suggestionId);

            if (suggestion == null) return null;

            return new SuggestionDto
            {
                Id = suggestion.Id,
                AnalyserUsername = suggestion.AnalyserUsername,
                TargetUsername = suggestion.TargetUsername,
                Content = suggestion.Content,
                ReportPeriodStart = suggestion.ReportPeriodStart,
                ReportPeriodEnd = suggestion.ReportPeriodEnd,
                IsRead = suggestion.IsRead,
                CreatedAt = suggestion.CreatedAt
            };
        }
    }
}