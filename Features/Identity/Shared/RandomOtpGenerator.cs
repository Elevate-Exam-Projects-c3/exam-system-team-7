using System.Security.Cryptography;

namespace exam_system.Features.Identity.Shared;

public class RandomOtpGenerator : IOtpGenerator
{
    public string GenerateSixDigitOtp()
    {
        // RandomNumberGenerator is the CRYPTOGRAPHIC random source. Unlike
        // new Random(), its output cannot be predicted from earlier values.
        // GetInt32 picks 0..999,999 with a uniform distribution — no modulo
        // bias like random.Next() % 1000000 would have.
        var number = RandomNumberGenerator.GetInt32(0, 1_000_000);

        // "D6" pads with leading zeros: 4231 -> "004231". An OTP with
        // leading zeros is still a valid 6-digit OTP.
        return number.ToString("D6");
    }
}
