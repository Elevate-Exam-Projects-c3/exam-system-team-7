using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminCreateQuiz.Commands;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Handlers {
    public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, Guid> {


        private readonly IGenericRepository<Quiz> quizRepository; 
        private readonly IUnitOfWork unitOfWork; 
        public CreateQuizCommandHandler(IGenericRepository<Quiz> quizRepository, IUnitOfWork unitOfWork) {
            this.quizRepository = quizRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(CreateQuizCommand request, CancellationToken cancellationToken) {

            var quiz = new Quiz {

                DiplomaId = request.CreateQuiz.DiplomaId,
                Title = request.CreateQuiz.Title.Trim(),

                Instructions = request.CreateQuiz.Instructions,

                DurationMinutes = request.CreateQuiz.DurationMinutes,
                PassScore = request.CreateQuiz.PassScore ?? 60,

                MaxAttempts = request.CreateQuiz.MaxAttempts,

                Status = QuizStatus.Draft,

                StartDate = request.CreateQuiz.StartDate,

                EndDate = request.CreateQuiz.EndDate
            };

            await quizRepository.AddAsync(quiz);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return quiz.Id;
        }
    }
}
