namespace exam_system.Features.Identity.Shared;

// Abstraction over password hashing (Dependency Inversion + Strategy pattern).
// Register, Login and the future Password Reset flow all depend on this
// interface — never on the BCrypt library directly — so the hashing algorithm
// can be replaced in one place without touching any feature code.
public interface IPasswordHasher
{
    // Returns a self-contained bcrypt hash: version, work factor, salt and
    // digest are all embedded in the string, so we never store a salt separately.
    string Hash(string password);

    // Re-hashes the input using the salt read from storedHash, then compares
    // the digests. Returns false on mismatch — it never throws for a wrong password.
    bool Verify(string password, string storedHash);
}
