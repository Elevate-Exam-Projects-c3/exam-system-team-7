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

                Id= Guid.NewGuid(),

                DiplomaId = request.DiplomaId,

                Title = request.Title.Trim(),

                Instructions = request.Instructions,

                DurationMinutes = request.DurationMinutes,

                PassScore = request.PassScore ?? 60,

                MaxAttempts = request.MaxAttempts,

                Status = QuizStatus.Draft,

                StartDate = request.StartDate,

                EndDate = request.EndDate,

                CreatedAt = DateTime.UtcNow,
               
                IsDeleted = false
            };

            await quizRepository.AddAsync(quiz);

            var result = await unitOfWork.SaveChangesAsync(cancellationToken);

            if(result <= 0)
                return RequestResponse<Guid>.Fail("Quiz failed to Carete.");


            return RequestResponse<Guid>.Created(quiz.Id,"Quiz created successfully.");
        }
    }
}
