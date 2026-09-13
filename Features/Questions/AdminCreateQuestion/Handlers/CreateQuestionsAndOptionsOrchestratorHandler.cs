using exam_system.Features.Questions.AdminAddOptions.Commands;
using exam_system.Features.Questions.AdminCreateQuestion.Commands;
using exam_system.Features.Questions.AdminCreateQuestion.Orchestrators;
using exam_system.Features.Quizzes.GetQuiz.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.AdminCreateQuestion.Handlers {
    public class CreateQuestionsAndOptionsOrchestratorHandler : IRequestHandler<CreateQuestionsAndOptionsOrchestrator, RequestResponse<bool>> {

        private readonly IMediator mediator;
        public CreateQuestionsAndOptionsOrchestratorHandler(IMediator mediator) {
            this.mediator = mediator;
        }
     

        public async Task<RequestResponse<bool>> Handle(CreateQuestionsAndOptionsOrchestrator request, CancellationToken cancellationToken) {

            var quiz = await mediator.Send(new CheckIfQuizExistQuery(request.quizId));

            if (!quiz.Data)
                return RequestResponse<bool>.Fail(quiz.Message, StatusCodes.Status404NotFound);


            var result = await mediator.Send(new CreateQuestionCommand(
               request.quizId, request.Text, request.Explanation, request.OrderIndex, request.Options), cancellationToken);

            if (!result.Success) {
                return RequestResponse<bool>.Fail(result.Message, StatusCodes.Status417ExpectationFailed);
            }

            var questionId = result.Data;

            foreach (var option in request.Options) {
                var optionResult = await mediator.Send(new CreateOptionsCommand( questionId, option.OptionText,option.IsCorrect),cancellationToken);

                if (!optionResult.Success) 
                    return RequestResponse<bool>.Fail(optionResult.Message,optionResult.StatusCode);
                
            }         
             
            return RequestResponse<bool>.Ok(true, "Question and options created successfully.");
        }
    }
}
