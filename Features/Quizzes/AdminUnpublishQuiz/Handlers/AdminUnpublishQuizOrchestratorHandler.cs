using exam_system.Features.Quizzes.AdminUnpublishQuiz.Commands;
using exam_system.Features.Quizzes.AdminUnpublishQuiz.Orchestrators;
using exam_system.Features.Quizzes.AdminUnpublishQuiz.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminUnpublishQuiz.Handlers
{
    public class AdminUnpublishQuizOrchestratorHandler:IRequestHandler<AdminUnpublishQuizOrchestrator , RequestResponse<Guid>>
    {
        private readonly IMediator _mediator;

        public AdminUnpublishQuizOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResponse<Guid>> Handle(AdminUnpublishQuizOrchestrator request, CancellationToken cancellationToken)
        {
            var inprogressAttemptResult = await _mediator.Send(new HasInProgressAttemptsQuery(request.QuizId), cancellationToken).ConfigureAwait(false);
            if(!inprogressAttemptResult.Success)
                return RequestResponse<Guid>.Fail(inprogressAttemptResult.Message);
            return await _mediator.Send(new AdminUnpublishQuizCommand(request.QuizId), cancellationToken);
        }
    }
}
