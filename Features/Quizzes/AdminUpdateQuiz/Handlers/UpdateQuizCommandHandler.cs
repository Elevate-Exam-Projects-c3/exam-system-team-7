using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminUpdateQuiz.Commands;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminUpdateQuiz.Handlers {
    public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, Guid> {

        private readonly IGenericRepository<Quiz> quizRepository;
        private readonly IUnitOfWork unitOfWork;


        public UpdateQuizCommandHandler(IGenericRepository<Quiz> quizRepository , IUnitOfWork unitOfWork) {
            this.quizRepository = quizRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(UpdateQuizCommand request, CancellationToken cancellationToken) {

          var quiz = await quizRepository.GetByIdAsync(request.QuizId);
            if (quiz == null) 
                throw new Exception($"Quiz with ID {request.QuizId} not found.");
            
            quiz.Title = request.UpdateQuiz.Title ?? quiz.Title;

            quiz.DurationMinutes =request.UpdateQuiz.DurationMinutes ?? quiz.DurationMinutes;

            quiz.PassScore =
                request.UpdateQuiz.PassScore ?? quiz.PassScore;

            quiz.MaxAttempts =
                request.UpdateQuiz.MaxAttempts ?? quiz.MaxAttempts;

            quiz.UpdatedAt = DateTime.UtcNow;

            quizRepository.Update(quiz);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return quiz.Id;
        }
    }
}
