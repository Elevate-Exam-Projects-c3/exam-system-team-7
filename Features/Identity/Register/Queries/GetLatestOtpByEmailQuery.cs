using MediatR;
using exam_system.Features.Identity.Register.Dtos;

namespace exam_system.Features.Identity.Register.Queries;

// Newest code for an email; UnusedOnly targets the latest unused one.
public record GetLatestOtpByEmailQuery(string Email, bool UnusedOnly = false)
    : IRequest<OtpCodeDto?>;
