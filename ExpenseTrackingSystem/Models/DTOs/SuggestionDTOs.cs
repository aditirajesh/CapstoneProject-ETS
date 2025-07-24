using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackingSystem.Models.DTOs
{
    public class SuggestionAddDto
    {
        [Required]
        public string TargetUsername { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Content { get; set; }

        [Required]
        public DateTime ReportPeriodStart { get; set; }

        [Required]
        public DateTime ReportPeriodEnd { get; set; }
    }

    public class SuggestionDto
    {
        public int Id { get; set; }
        public string AnalyserUsername { get; set; }
        public string TargetUsername { get; set; }
        public string Content { get; set; }
        public DateTime ReportPeriodStart { get; set; }
        public DateTime ReportPeriodEnd { get; set; }
        public bool IsRead { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
    
}

