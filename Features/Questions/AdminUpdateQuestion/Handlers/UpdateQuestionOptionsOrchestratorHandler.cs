using exam_system.Features.Questions.AdminUpdateOptions.Commands;
using exam_system.Features.Questions.AdminUpdateQuestion.Commands;
using exam_system.Features.Questions.AdminUpdateQuestion.Orchestrators;
using exam_system.Features.Quizzes.GetQuiz.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Questions.AdminUpdateQuestion.Handlers {
    public class UpdateQuestionOptionsOrchestratorHandler: IRequestHandler<UpdateQuestionOptionsOrchestrator,RequestResponse<bool>> {
        private readonly IMediator mediator;
        private readonly IUnitOfWork unitOfWork;

        public UpdateQuestionOptionsOrchestratorHandler(IMediator mediator,IUnitOfWork unitOfWork) {

            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse<bool>> Handle(UpdateQuestionOptionsOrchestrator request,CancellationToken cancellationToken) {

            await unitOfWork.BeginTransactionAsync();

            try {
                

                var updateQuestion = await mediator.Send(
                    new UpdateQuestionCommand(
                        request.QuestionId,
                        request.Text,
                        request.Explanation,
                        request.OrderIndex),
                    cancellationToken);

                if (!updateQuestion.Success) {
                    await unitOfWork.RollbackTransactionAsync();

                    return RequestResponse<bool>.Fail(updateQuestion.Message,updateQuestion.StatusCode);
                }

                foreach (var option in request.Options) {
                    var updateOptionResponse =
                        await mediator.Send(new UpdateOptionsCommand(request.QuestionId,option.Id,option.OptionText,option.IsCorrect),cancellationToken);

                    if (!updateOptionResponse.Success) {

                        await unitOfWork.RollbackTransactionAsync();

                        return RequestResponse<bool>.Fail(updateOptionResponse.Message,updateOptionResponse.StatusCode);
                    }
                }

                var saveResult =await unitOfWork.SaveChangesAsync(cancellationToken);

                if (saveResult <= 0) {
                    await unitOfWork.RollbackTransactionAsync();

                    return RequestResponse<bool>.Fail("Failed to update question and options.",StatusCodes.Status500InternalServerError);
                }

                await unitOfWork.CommitTransactionAsync();

                return RequestResponse<bool>.Ok(true,"Question and options updated successfully.");

            } catch (Exception ex) {
                await unitOfWork.RollbackTransactionAsync();

                return RequestResponse<bool>.Fail(ex.Message,StatusCodes.Status500InternalServerError);
            }
        }
    }
}