using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Commands;

// EXAM-105: single-object mutation — increment ONE OtpCodes row's
// AttemptCount after a wrong submission. The Orchestrator sends it only on
// the wrong-code path (never on expired/locked/missing — see the gates).
// Data of the response = the NEW AttemptCount, so the Orchestrator can decide
// the response message (locked at 5 vs generic invalid).
public record RecordWrongOtpAttemptCommand(Guid OtpId)
    : IRequest<RequestResponse<int>>;
