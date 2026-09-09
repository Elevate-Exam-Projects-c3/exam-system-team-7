using MediatR;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Handlers;

// TEMPORARY stub — exists only so MediatR can route RegisterUserCommand while we
// test the validation pipeline. EXAM-103 replaces the body with the real flow:
// check email uniqueness (409), hash with bcrypt, save user, trigger OTP.
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RequestResponse<Guid>>
{
    public Task<RequestResponse<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // If you see this message in Swagger, the validator passed and the
        // pipeline let the request reach the Handler.
        return Task.FromResult(RequestResponse<Guid>.Ok(Guid.NewGuid(), "Validation passed - Handler reached."));
    }
}
