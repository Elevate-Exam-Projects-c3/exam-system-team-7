using exam_system.Common.Enums;
using exam_system.Features.Questions.AdminDeleteQuestion.Commands;
using exam_system.Features.Questions.AdminDeleteQuestion.Orchestrators;
using exam_system.Features.Quizzes.AdminGetQuiz.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.AdminDeleteQuestion.Handlers {
    public class DeleteQuestionOrchestratorHandler : IRequestHandler<DeleteQuestionOrchestrator, RequestResponse<bool>> {

        private readonly IMediator mediator;

        public DeleteQuestionOrchestratorHandler(IMediator mediator) {
            this.mediator = mediator;
        }

        public async Task<RequestResponse<bool>> Handle(DeleteQuestionOrchestrator request, CancellationToken cancellationToken) {

            var quiz = await mediator.Send(new GetQuizDetailsQuery(request.quizId), cancellationToken);
            if (quiz == null || !quiz.Success) 
                return RequestResponse<bool>.Fail("Quiz not found");
            
            if(quiz.Data.Status == QuizStatus.Published) 
                return RequestResponse<bool>.Fail("Cannot delete question from a published quiz", StatusCodes.Status409Conflict);

            var deleteQuestion = await mediator.Send(new DeleteQuestionCommand(request.questionId), cancellationToken);
            if (!deleteQuestion.Success) 
                return RequestResponse<bool>.Fail(deleteQuestion.Message);

            return RequestResponse<bool>.Ok(true , deleteQuestion.Message);
        }
    }
}
