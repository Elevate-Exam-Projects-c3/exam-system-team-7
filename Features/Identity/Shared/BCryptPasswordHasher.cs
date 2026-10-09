namespace exam_system.Features.Identity.Shared;

// bcrypt password hashing; the only file that talks to the BCrypt library.
public class BCryptPasswordHasher : IPasswordHasher
{
    // 2^12 rounds — the OWASP-recommended bcrypt cost.
    private const int WorkFactor = 12;

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
    }

    public bool Verify(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}
