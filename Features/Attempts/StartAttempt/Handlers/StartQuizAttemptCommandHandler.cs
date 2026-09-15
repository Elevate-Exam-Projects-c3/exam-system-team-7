using exam_system.Common.Enums;
using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.StartAttempt.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Attempts.StartAttempt.Handlers {
    public class StartQuizAttemptCommandHandler : IRequestHandler<StartQuizAttemptCommand, RequestResponse<Guid>> {

        private readonly IGenericRepository<QuizAttempt> quizAttemptRepoitory;
        private readonly IUnitOfWork unitOfWork;

        public StartQuizAttemptCommandHandler(IGenericRepository<QuizAttempt> quizAttemptRepoitory , IUnitOfWork unitOfWork) {
            this.quizAttemptRepoitory = quizAttemptRepoitory;
            this.unitOfWork = unitOfWork;
        }
        public async Task<RequestResponse<Guid>> Handle(StartQuizAttemptCommand request, CancellationToken cancellationToken) {

            Console.WriteLine($"StudentId: {request.StudentId}");
            Console.WriteLine($"QuizId: {request.QuizId}");
            Console.WriteLine($"Duration: {request.DurationMinutes}");


            var newAttempt = new QuizAttempt {
                QuizId = request.QuizId,
                StudentId = request.StudentId,
                Status = AttemptStatus.InProgress,
                Deadline = DateTime.UtcNow.AddMinutes(request.DurationMinutes)
            };
            await quizAttemptRepoitory.AddAsync(newAttempt);

            var result = await unitOfWork.SaveChangesAsync(cancellationToken);

            if (result <= 0)
                return RequestResponse<Guid>.Fail("Attempt failed to create.");


            return RequestResponse<Guid>.Created(newAttempt.Id, "Attempt created successfully.");

        }
    }
}
