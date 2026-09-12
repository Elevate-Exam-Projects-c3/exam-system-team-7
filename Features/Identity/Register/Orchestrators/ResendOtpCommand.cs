using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Orchestrators;

// EXAM-2 internal subtask (team decision, 2026-09-12): the stories demand a
// resend ("requiring a resend" in the lockout rule) but define no endpoint
// for it. This is the flow-entry request; it is handled by the ORCHESTRATOR
// (convention #10) even though the flow issues ONE command, because it
// coordinates several steps (read user, read latest OTP, issue, notify).
// Non-generic RequestResponse: the reply is a NEUTRAL message with no data —
// returning the user id would reveal whether the email exists.
public record ResendOtpCommand(string Email)
    : IRequest<RequestResponse>;
