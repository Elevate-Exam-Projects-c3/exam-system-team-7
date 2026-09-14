namespace exam_system.Features.Identity.Shared;

// Generates 6-digit verification codes.
public interface IOtpGenerator
{
    // Exactly 6 digits, leading zeros included (e.g. "004231").
    string GenerateSixDigitOtp();
}
