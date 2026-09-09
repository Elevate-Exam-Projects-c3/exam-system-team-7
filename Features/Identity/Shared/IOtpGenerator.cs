namespace exam_system.Features.Identity.Shared;

// Generates one-time codes. An OTP is a SECURITY value: it must be
// unpredictable, so the implementation has to use a cryptographic
// random source — never the ordinary Random class.
public interface IOtpGenerator
{
    // Returns EXACTLY 6 digits, e.g. "004231" — leading zeros are valid.
    string GenerateSixDigitOtp();
}
