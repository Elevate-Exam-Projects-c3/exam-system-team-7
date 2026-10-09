using exam_system.Common.Enums;
using exam_system.Dtos.Attempts;
using exam_system.Features.Attempts.CheckRemainingTime.Commands;
using exam_system.Features.Attempts.CheckRemainingTime.Orchestrators;
using exam_system.Features.Attempts.GetOldAttempt.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.CheckRemainingTime.Handlers {
    public class GetAttemptTimeRemainingOrchestratorHandler : IRequestHandler<GetAttemptTimeRemainingOrchestrator, RequestResponse<AttemptTimeRemainingDto>> {

        private readonly IMediator mediator;

        public GetAttemptTimeRemainingOrchestratorHandler(IMediator mediator) {
            this.mediator = mediator;
      }
        public async Task<RequestResponse<AttemptTimeRemainingDto>> Handle(GetAttemptTimeRemainingOrchestrator request, CancellationToken cancellationToken) {

            var attemptResult = await mediator.Send(new GetOldQuizAttemptQuery(request.attemptId), cancellationToken);

            if (!attemptResult.Success || attemptResult.Data == null) {
                return RequestResponse<AttemptTimeRemainingDto>.Fail(attemptResult.Message, attemptResult.StatusCode, attemptResult.Errors);
            }

            var attempt = attemptResult.Data;

            var timeNow = DateTime.UtcNow;

            if (attemptResult.Data.Status == AttemptStatus.TimedOut) {

                var timedOutResponse = new AttemptTimeRemainingDto {
                    AttemptId = request.attemptId,
                    ServerTimeUtc = timeNow,
                    Deadline = attempt.Deadline,
                    RemainingSeconds = 0,
                    Status = AttemptStatus.TimedOut
                };
                return RequestResponse<AttemptTimeRemainingDto>.Ok(timedOutResponse,"Attempt has timed out.");
            }


            if (timeNow > attempt.Deadline) {
       var expiredResponse = new AttemptTimeRemainingDto {
                    AttemptId = request.attemptId,
                    ServerTimeUtc = timeNow,
                    Deadline = attempt.Deadline,
                    RemainingSeconds = 0,
                    Status = AttemptStatus.TimedOut
                };

                return RequestResponse<AttemptTimeRemainingDto>.Ok(expiredResponse, "Attempt deadline has expired.");

            }

            var remainingSeconds =(long)Math.Ceiling((attempt.Deadline - timeNow).TotalSeconds);

            var response = new AttemptTimeRemainingDto {
                AttemptId = request.attemptId,
                ServerTimeUtc = timeNow,
                Deadline = attempt.Deadline,
              RemainingSeconds = Math.Max(0,remainingSeconds),
                Status = attempt.Status
            };

            return RequestResponse<AttemptTimeRemainingDto>.Ok(response,"Remaining time retrieved successfully.");

        }
    }
}
