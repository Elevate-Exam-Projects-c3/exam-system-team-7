using exam_system.Features.Diplomas.CommonQueries.Queries;
using exam_system.Features.Diplomas.GetDiplomaDetail.Queries;
using exam_system.Features.Quizzes.AdminCreateQuiz.Commands;
using exam_system.Features.Quizzes.AdminCreateQuiz.Orchestrators;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Handlers {
    public class CreateQuizOrchestratorHandler : IRequestHandler<CreateQuizOrchestrator, RequestResponse<Guid>> {

       private readonly IMediator mediator;

        public CreateQuizOrchestratorHandler(IMediator mediator) {
            this.mediator = mediator;

        }
        public async Task<RequestResponse<Guid>> Handle(CreateQuizOrchestrator request, CancellationToken cancellationToken) {

            var diploma =  await mediator.Send(new CheckDiplomaExistsQuery(request.DiplomaId));

            if (diploma == false)
                    return RequestResponse<Guid>.Fail("Diploma not found", StatusCodes.Status404NotFound);

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
                return RequestResponse<Guid>.Fail(result.Message, StatusCodes.Status417ExpectationFailed);


            }
            return RequestResponse<Guid>.Ok(result.Data, result.Message);
        }
    }
}
