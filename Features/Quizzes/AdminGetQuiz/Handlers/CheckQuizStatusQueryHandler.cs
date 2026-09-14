using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminGetQuiz.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Common.Enums;
namespace exam_system.Features.Quizzes.AdminGetQuiz.Handlers {
    public class CheckQuizStatusQueryHandler : IRequestHandler<CheckQuizStatusQuery, RequestResponse<bool>> {

        private readonly IGenericRepository<Quiz> quizRepository;

        public CheckQuizStatusQueryHandler(IGenericRepository<Quiz> quizRepository) {
            this.quizRepository = quizRepository;
        }
        public async Task<RequestResponse<bool>> Handle(CheckQuizStatusQuery request, CancellationToken cancellationToken) {

            var quiz = await quizRepository.GetAll().FirstOrDefaultAsync(x=> x.Id == request.QuizId && !x.IsDeleted);

            if (quiz == null)
                return RequestResponse<bool>.Fail("Quiz not found." , StatusCodes.Status404NotFound);

            if (quiz?.Status != QuizStatus.Published) 
                  return RequestResponse<bool>.Fail("Quiz is not available. Only published quizzes can be started.",StatusCodes.Status409Conflict);        

            return RequestResponse<bool>.Ok(true,"Quiz is available." );
            }
    }
}
