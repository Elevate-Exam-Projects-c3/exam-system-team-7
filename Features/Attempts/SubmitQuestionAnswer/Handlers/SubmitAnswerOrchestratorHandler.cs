using exam_system.Common.Enums;
using exam_system.Features.Attempts.CheckRemainingTime.Commands;
using exam_system.Features.Attempts.GetOldAttempt.Queries;
using exam_system.Features.Attempts.SubmitQuestionAnswer.Orchestrators;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Handlers {
    public class SubmitAnswerOrchestratorHandler : IRequestHandler<SubmitAnswerOrchestrator, RequestResponse<bool>> {

        private readonly IMediator mediator;

        public SubmitAnswerOrchestratorHandler(IMediator mediator) {
            this.mediator = mediator;
        }
        public async Task<RequestResponse<bool>> Handle(SubmitAnswerOrchestrator request, CancellationToken cancellationToken) {

            var attemptResult = await mediator.Send(new GetOldQuizAttemptQuery( request.AttemptId), cancellationToken);

            if(attemptResult == null || !attemptResult.Success) 
                return RequestResponse<bool>.Fail(attemptResult.Message, attemptResult.StatusCode);

            var attempt = attemptResult.Data;

            if (attempt?.Status == AttemptStatus.TimedOut)
                return RequestResponse<bool>.Fail("Attempt has timed out. Answer cannot be submitted.",StatusCodes.Status410Gone);

            var now = DateTime.Now;
            Console.WriteLine($"Current time: {now}");

            if (now > attempt?.Deadline) {
                var timeoutResult = await mediator.Send(new TimeoutAttemptCommand(request.AttemptId), cancellationToken);
                if (!timeoutResult.Success) 
                    return RequestResponse<bool>.Fail(timeoutResult.Message, timeoutResult.StatusCode);          
                return RequestResponse<bool>.Fail("Attempt has timed out. Answer was not submitted.",StatusCodes.Status410Gone);
            }


            throw new NotImplementedException();
        }


    }
}
