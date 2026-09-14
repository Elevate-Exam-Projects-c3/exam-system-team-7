using System.Security.Cryptography;

namespace exam_system.Features.Identity.Shared;

public class RandomOtpGenerator : IOtpGenerator
{
    public string GenerateSixDigitOtp()
    {
        // Crypto-random source: uniform and unpredictable.
        var number = RandomNumberGenerator.GetInt32(0, 1_000_000);

        // Pad so "4231" becomes "004231".
        return number.ToString("D6");
    }
}
