namespace exam_system.Features.Identity.Shared;

// The only place in the app that talks to the BCrypt library.
// Stateless and thread-safe, which is why it is registered as a Singleton.
public class BCryptPasswordHasher : IPasswordHasher
{
    // Work factor 12 = 2^12 rounds, roughly 200-400 ms per hash on normal
    // hardware: slow enough to make brute-force guessing expensive, fast
    // enough that a user never notices. EXAM-102 requires salt rounds >= 12.
    private const int WorkFactor = 12;

    public string Hash(string password)
    {
        // The salt is generated inside the library and embedded in the result,
        // so hashing the same password twice gives two different hashes.
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
    }

    public bool Verify(string password, string storedHash)
    {
        // The library reads the work factor and salt back out of storedHash,
        // repeats the exact same computation, and compares the digests.
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}
