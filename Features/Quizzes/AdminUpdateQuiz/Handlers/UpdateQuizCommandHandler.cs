using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminUpdateQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminUpdateQuiz.Handlers {
    public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, RequestResponse<Guid>> {

        private readonly IGenericRepository<Quiz> quizRepository;
        private readonly IUnitOfWork unitOfWork;


        public UpdateQuizCommandHandler(IGenericRepository<Quiz> quizRepository , IUnitOfWork unitOfWork) {
            this.quizRepository = quizRepository;
            this.unitOfWork = unitOfWork;
        }
      

        async Task<RequestResponse<Guid>> IRequestHandler<UpdateQuizCommand, RequestResponse<Guid>>.Handle(UpdateQuizCommand request, CancellationToken cancellationToken) {
           
            var quiz = await quizRepository.GetAll().FirstOrDefaultAsync( x => x.Id == request.QuizId && !x.IsDeleted,  cancellationToken);

            if (quiz is null) {
                return RequestResponse<Guid>.Fail("Quiz not found.",StatusCodes.Status404NotFound);
            }
           

            quiz.Title = request.Title ?? quiz.Title;

            quiz.DurationMinutes = request.DurationMinutes ?? quiz.DurationMinutes;

            quiz.PassScore = request.PassScore ?? quiz.PassScore;

            quiz.MaxAttempts = request.MaxAttempts ?? quiz.MaxAttempts;

            quiz.StartDate = request.StartDate ?? quiz.StartDate;

            quiz.EndDate = request.EndDate ?? quiz.EndDate;

            quiz.UpdatedAt = DateTime.UtcNow;

            quizRepository.Update(quiz);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Guid>.Ok(quiz.Id, "Quiz updated successfully.");
        }
    }
}
