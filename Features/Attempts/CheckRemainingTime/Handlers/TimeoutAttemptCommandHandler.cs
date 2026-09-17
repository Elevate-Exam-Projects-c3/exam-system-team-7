using exam_system.Common.Enums;
using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.CheckRemainingTime.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.CheckRemainingTime.Handlers {
        public class TimeoutAttemptCommandHandler : IRequestHandler<TimeoutAttemptCommand, RequestResponse<bool>> {

            private readonly IGenericRepository<QuizAttempt> quizAttemptRepository;
            private readonly IUnitOfWork unitOfWork;


            public TimeoutAttemptCommandHandler(IGenericRepository<QuizAttempt> quizAttemptRepository, IUnitOfWork unitOfWork) {
                this.quizAttemptRepository = quizAttemptRepository;
                this.unitOfWork = unitOfWork;
            }

            public async Task<RequestResponse<bool>> Handle(TimeoutAttemptCommand request, CancellationToken cancellationToken) {

                var attempt = await quizAttemptRepository.GetAll().FirstOrDefaultAsync(a => a.Id == request.attemptId && !a.IsDeleted, cancellationToken);

                if (attempt == null)
                    return RequestResponse<bool>.Fail("Attempt not found.", StatusCodes.Status404NotFound);

                attempt.Status = AttemptStatus.TimedOut;

                await quizAttemptRepository.UpdateAsync(attempt);

                var saveResult = await unitOfWork.SaveChangesAsync(cancellationToken);

                if (saveResult <= 0)
                    return RequestResponse<bool>.Fail("Failed to update attempt status.", StatusCodes.Status500InternalServerError);

                return RequestResponse<bool>.Ok(true, "Attempt timed out successfully.", StatusCodes.Status200OK);

            }
        }
}
