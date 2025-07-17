using System;

namespace ExpenseTrackingSystem.Models.DTOs;

public class EmailReportRequestDto
{
    public string Email { get; set; } = string.Empty;
    public int LastNDays { get; set; } = 30;
}
