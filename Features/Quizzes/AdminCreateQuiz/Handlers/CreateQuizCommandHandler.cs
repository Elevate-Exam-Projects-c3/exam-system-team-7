using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminCreateQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Handlers {
    public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, RequestResponse<Guid>> {


        private readonly IGenericRepository<Quiz> quizRepository;
        private readonly IUnitOfWork unitOfWork;

        public CreateQuizCommandHandler(IGenericRepository<Quiz> quizRepository, IUnitOfWork unitOfWork) {
            this.quizRepository = quizRepository;
            this.unitOfWork = unitOfWork;
        }

        async Task<RequestResponse<Guid>> IRequestHandler<CreateQuizCommand, RequestResponse<Guid>>.Handle(CreateQuizCommand request, CancellationToken cancellationToken) {
            var quiz = new Quiz {

                DiplomaId = request.DiplomaId,

                Title = request.Title.Trim(),

                Instructions = request.Instructions,

                DurationMinutes = request.DurationMinutes,

                PassScore = request.PassScore ?? 60,

                MaxAttempts = request.MaxAttempts,

                Status = QuizStatus.Draft,

                StartDate = request.StartDate,

                EndDate = request.EndDate
            };

            await quizRepository.AddAsync(quiz);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Guid>.Created(quiz.Id,"Quiz created successfully.");
        }
    }
}
