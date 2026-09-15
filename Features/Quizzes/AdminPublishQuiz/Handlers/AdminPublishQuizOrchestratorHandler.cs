using exam_system.Features.Quizzes.AdminPublishQuiz.Commands;
using exam_system.Features.Quizzes.AdminPublishQuiz.Orchestrators;
using exam_system.Features.Quizzes.AdminQuizPublishCheck.Orchestrators;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminPublishQuiz.Handlers
{
    public class AdminPublishQuizOrchestratorHandler : IRequestHandler<AdminPublishQuizOrchestrator, RequestResponse<Guid>>
    {
        private readonly IMediator _mediator;

        public AdminPublishQuizOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<RequestResponse<Guid>> Handle(AdminPublishQuizOrchestrator request, CancellationToken cancellationToken)
        {
            var checkResult = await _mediator.Send(new QuizPublishCheckListOrchestrator(request.quizId), cancellationToken);
            var failedChecks = checkResult.Data.Checks.Where(c=>!c.IsPassed);

            if (failedChecks.Count() > 0)
            {
                return RequestResponse<Guid>.Fail("Quiz cannot be published due to failed checks: " + string.Join(", ", failedChecks.Select(c => c.Message)));
            }
            return await _mediator.Send(new AdminPublishQuizCommand(request.quizId), cancellationToken);

        }
    }
}
