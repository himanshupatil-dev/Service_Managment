using System.ComponentModel.DataAnnotations;

namespace Service_Managment.Application.DTOs;

public sealed class StartJobRequest
{
    [Required, RegularExpression(@"^\d{6}$")]
    public string Otp { get; set; } = string.Empty;
}

public sealed class FailureReasonRequest
{
    [Required, StringLength(500, MinimumLength = 5)]
    public string FailureReason { get; set; } = string.Empty;
}
