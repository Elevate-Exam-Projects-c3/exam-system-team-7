using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.GetQuiz.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.GetQuiz.Handlers {
    public class CheckIfQuizExistQueryHandler: IRequestHandler<CheckIfQuizExistQuery, bool> {

        private readonly IGenericRepository<Quiz> quizRepository;

        public CheckIfQuizExistQueryHandler(IGenericRepository<Quiz> quizRepository) {
            this.quizRepository = quizRepository;
        }

        public async Task<bool> Handle(CheckIfQuizExistQuery request, CancellationToken cancellationToken) {
            return await quizRepository.GetAll().AnyAsync(q => q.Id == request.QuizId,cancellationToken);
        }

        
    }
}
