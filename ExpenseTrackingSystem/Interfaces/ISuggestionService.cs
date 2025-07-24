using System;
using ExpenseTrackingSystem.Models.DTOs;

namespace ExpenseTrackingSystem.Interfaces;

public interface ISuggestionService
    {
        /// <summary>
        /// Creates a new suggestion and saves it to the database.
        /// </summary>
        Task<SuggestionDto> CreateSuggestionAsync(string analyserUsername, SuggestionAddDto suggestionDto);

        /// <summary>
        /// Retrieves all suggestions for a specific user.
        /// </summary>
        Task<IEnumerable<SuggestionDto>> GetSuggestionsForUserAsync(string targetUsername);
        
        /// <summary>
        /// Marks a suggestion as read, if the request comes from the correct user.
        /// </summary>
        Task MarkSuggestionAsReadAsync(int suggestionId, string username);
        
        /// <summary>
        /// Retrieves a single suggestion by its ID.
        /// </summary>
        Task<SuggestionDto?> GetSuggestionByIdAsync(int suggestionId);
    }
