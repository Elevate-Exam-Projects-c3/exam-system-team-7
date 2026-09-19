using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.GetOldAttempt.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.GetOldAttempt.Handlers {
    public class CheckMaxAttemptsQueryHandler : IRequestHandler<CheckMaxAttemptsQuery, RequestResponse<bool>> {

        private readonly IGenericRepository<QuizAttempt> attemptRepository;
        public CheckMaxAttemptsQueryHandler(IGenericRepository<QuizAttempt> attemptRepository) {
            this.attemptRepository = attemptRepository;
        }
        public async Task<RequestResponse<bool>> Handle(CheckMaxAttemptsQuery request, CancellationToken cancellationToken) {
            
            if (!request.MaxAttempts.HasValue) {
                return RequestResponse<bool>.Ok(true, "No max attempts limit.");
            }

            var attemptsCount = await attemptRepository.GetAll()
                .CountAsync(a =>
                            a.QuizId == request.QuizId && 
                            a.StudentId == request.StudentId && 
                            (
                                a.Status == Common.Enums.AttemptStatus.Submitted ||
                                a.Status == Common.Enums.AttemptStatus.TimedOut

                            ));

            if (attemptsCount >= request.MaxAttempts.Value) 
                return RequestResponse<bool>.Fail("Max attempts reached.", StatusCodes.Status409Conflict);


            return RequestResponse<bool>.Ok(true,"Attempts available.");

        }
    }
}
