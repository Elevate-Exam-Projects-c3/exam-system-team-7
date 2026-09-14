using exam_system.Features.Questions.AdminAddOptions.Commands;
using exam_system.Features.Questions.AdminCreateQuestion.Commands;
using exam_system.Features.Questions.AdminCreateQuestion.Orchestrators;
using exam_system.Features.Quizzes.GetQuiz.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Questions.AdminCreateQuestion.Handlers {
    public class CreateQuestionsAndOptionsOrchestratorHandler : IRequestHandler<CreateQuestionsAndOptionsOrchestrator, RequestResponse<bool>> {

        private readonly IMediator mediator;
        private readonly IUnitOfWork unitOfWork;
        public CreateQuestionsAndOptionsOrchestratorHandler(IMediator mediator, IUnitOfWork unitOfWork) {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
        }
     

        public async Task<RequestResponse<bool>> Handle(CreateQuestionsAndOptionsOrchestrator request, CancellationToken cancellationToken) {

            await unitOfWork.BeginTransactionAsync();

            try {

                var quiz = await mediator.Send(new CheckIfQuizExistQuery(request.quizId));

                if (!quiz.Data) {
                    await unitOfWork.RollbackTransactionAsync();
                    return RequestResponse<bool>.Fail(quiz.Message, StatusCodes.Status404NotFound);
                }

                var question = await mediator.Send(new CreateQuestionCommand(
                   request.quizId, request.Text, request.Explanation, request.OrderIndex, request.Options), cancellationToken);


                if (!question.Success) {
                    await unitOfWork.RollbackTransactionAsync();
                    return RequestResponse<bool>.Fail(question.Message, StatusCodes.Status417ExpectationFailed);
                }

                var questionId = question.Data;

                foreach (var option in request.Options) {
                    var optionResult = await mediator.Send(new CreateOptionsCommand(questionId, option.OptionText, option.IsCorrect), cancellationToken);

                    if (!optionResult.Success) {
                        await unitOfWork.RollbackTransactionAsync();
                        return RequestResponse<bool>.Fail(optionResult.Message, optionResult.StatusCode);
                    }
                }

              var result = await unitOfWork.SaveChangesAsync();

                if (result <= 0) {
                    await unitOfWork.RollbackTransactionAsync();
                    return RequestResponse<bool>.Fail("Fail to save Questions and Options.", StatusCodes.Status500InternalServerError );
                }

              await unitOfWork.CommitTransactionAsync();

                return RequestResponse<bool>.Created(true,"Question and options created successfully.");
          
            } catch {

                await unitOfWork.RollbackTransactionAsync();
                return RequestResponse<bool>.Fail("Fail to save Questions and Options");

            }
        }
    }
}
