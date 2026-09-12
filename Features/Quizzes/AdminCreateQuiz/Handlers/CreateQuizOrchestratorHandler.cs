using exam_system.Features.Diplomas.GetDiplomaDetail.Queries;
using exam_system.Features.Quizzes.AdminCreateQuiz.Commands;
using exam_system.Features.Quizzes.AdminCreateQuiz.Orchestrators;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Handlers {
    public class CreateQuizOrchestratorHandler : IRequestHandler<CreateQuizOrchestrator, RequestResponse<bool>> {

       private readonly IMediator mediator;

        public CreateQuizOrchestratorHandler(IMediator mediator) {
            this.mediator = mediator;

        }
        public async Task<RequestResponse<bool>> Handle(CreateQuizOrchestrator request, CancellationToken cancellationToken) {

            var diploma =  await mediator.Send(new CheckIfDiplomaExistQuery(request.DiplomaId));

            if (!diploma.Data)
                    throw new KeyNotFoundException($"Diploma with ID {request.DiplomaId} not found.");

           var result =  await mediator.Send(new CreateQuizCommand(
                request.DiplomaId,
                request.Title,
                request.Instructions,
                request.DurationMinutes,
                request.PassScore,
                request.MaxAttempts,
                request.StartDate,
                request.EndDate
                ));
            if (!result.Success) {
                throw new InvalidOperationException(result.Message);
            }
            return RequestResponse<bool>.Ok(true, result.Message);
        }
    }
}
