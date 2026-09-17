using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminManageQuestions.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Handlers {
    public class ValidateQuestionForQuizQueryHandler : IRequestHandler<ValidateQuestionForQuizQuery, RequestResponse<bool>> {

        private readonly IGenericRepository<Question> questionsRepository;

        public ValidateQuestionForQuizQueryHandler(IGenericRepository<Question> questionsRepository) {
             this.questionsRepository = questionsRepository;
        }
        public  async Task<RequestResponse<bool>> Handle(ValidateQuestionForQuizQuery request, CancellationToken cancellationToken) {

             var questionExists = await questionsRepository.GetAll().AsNoTracking()
                .AnyAsync(q =>q.Id == request.QuestionId &&q.QuizId == request.QuizId &&!q.IsDeleted,cancellationToken);

            if (!questionExists) 
                return RequestResponse<bool>.Fail("Question does not exist for the specified quiz.", StatusCodes.Status404NotFound);

            return RequestResponse<bool>.Ok(true, "Question is valid for the specified quiz.");
        }
    }
}
