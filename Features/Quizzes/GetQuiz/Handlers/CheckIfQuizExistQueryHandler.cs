using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.GetQuiz.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.GetQuiz.Handlers {
    public class CheckIfQuizExistQueryHandler: IRequestHandler<CheckIfQuizExistQuery, RequestResponse<bool>> {

        private readonly IGenericRepository<Quiz> quizRepository;

        public CheckIfQuizExistQueryHandler(IGenericRepository<Quiz> quizRepository) {
            this.quizRepository = quizRepository;
        }

     

        async Task<RequestResponse<bool>> IRequestHandler<CheckIfQuizExistQuery, RequestResponse<bool>>.Handle(CheckIfQuizExistQuery request, CancellationToken cancellationToken) {
            return RequestResponse<bool>.Ok( await quizRepository.GetAll().AnyAsync(q => q.Id == request.QuizId && !q.IsDeleted, cancellationToken),
                "Quiz existence checked successfully."
            );
        }
    }
}
