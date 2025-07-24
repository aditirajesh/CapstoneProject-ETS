using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackingSystem.Models;

public class Suggestion
{
        [Key]
        public int Id { get; set; }

        [Required]
        public string AnalyserUsername { get; set; }

        [Required]
        public string TargetUsername { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime ReportPeriodStart { get; set; }

        [Required]
        public DateTime ReportPeriodEnd { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

}
