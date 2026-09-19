using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Questions.AdminUpdateQuestion.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Questions.AdminUpdateQuestion.Handlers {
    public class UpdateQuestionCommandHandler: IRequestHandler<UpdateQuestionCommand, RequestResponse<Guid>> {

        private readonly IGenericRepository<Question> questionRepository;

        public UpdateQuestionCommandHandler(IGenericRepository<Question> questionRepository) {

            this.questionRepository = questionRepository;
        }

        public async Task<RequestResponse<Guid>> Handle(UpdateQuestionCommand request,CancellationToken cancellationToken) {

            var question = questionRepository
                .GetAll()
                .FirstOrDefault(q =>
                    q.Id == request.QuestionId &&
                    !q.IsDeleted);

            if (question == null) 
                return RequestResponse<Guid>.Fail("Question not found.",StatusCodes.Status404NotFound);
            

            question.Text = request.Text.Trim();

            question.Explanation =
                request.Explanation?.Trim();

            question.OrderIndex =
                request.OrderIndex;

            questionRepository.Update(question);

            return RequestResponse<Guid>.Ok(question.Id,"Question updated successfully.");
        }
    }
}