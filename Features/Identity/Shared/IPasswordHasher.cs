namespace exam_system.Features.Identity.Shared;

// Password hashing abstraction; implementation: BCryptPasswordHasher.
public interface IPasswordHasher
{
    // Returns a self-contained bcrypt hash (salt embedded).
    string Hash(string password);

    // True when the password matches the stored hash.
    bool Verify(string password, string storedHash);
}
