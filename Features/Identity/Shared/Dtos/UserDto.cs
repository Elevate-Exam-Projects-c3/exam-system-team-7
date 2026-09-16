using exam_system.Common.Enums;

namespace exam_system.Features.Identity.Shared.Dtos;

// User fields shared by the Identity flows; no entity leaves queries.
public record UserDto(
    Guid Id,
    string Email,
    string PasswordHash,
    UserRole Role,
    AccountStatus AccountStatus,
    bool EmailConfirmed,
    DateTime? LockoutEnd);
