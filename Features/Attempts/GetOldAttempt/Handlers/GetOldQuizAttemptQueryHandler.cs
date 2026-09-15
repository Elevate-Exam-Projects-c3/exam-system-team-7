using exam_system.Dtos.Attempts;
using exam_system.Features.Attempts.GetOldAttempt.Queries;
using exam_system.Features.Shared;
using exam_system.Domain.Entities.Attempts;
using exam_system.Persistence.DataAccess;
using Microsoft.EntityFrameworkCore;
using MediatR;
namespace exam_system.Features.Attempts.GetOldAttempt.Handlers {
    public class GetOldQuizAttemptQueryHandler : IRequestHandler<GetOldQuizAttemptQuery, RequestResponse<QuizAttemptDto>> {

        private readonly IGenericRepository<QuizAttempt> quizAttemptRepoitory;
        public GetOldQuizAttemptQueryHandler(IGenericRepository<QuizAttempt> quizAttemptRepoitory) {
            this.quizAttemptRepoitory = quizAttemptRepoitory;
        }
        public async Task<RequestResponse<QuizAttemptDto>> Handle(GetOldQuizAttemptQuery request, CancellationToken cancellationToken) {

            var attempt =await quizAttemptRepoitory.GetAll()
                .Where(a => a.Id == request.attemptId)
                .Select(a => new QuizAttemptDto {
                    QuizId = a.QuizId,
                    StartTime = a.StartTime,
                    Deadline = a.Deadline,
                    SubmittedAt = a.SubmittedAt,
                    Score = a.Score,
                    Passed = a.Passed
                }).FirstOrDefaultAsync(cancellationToken);

            if (attempt == null) 
                return RequestResponse<QuizAttemptDto>.Fail("Attempt not found.", StatusCodes.Status404NotFound); 


            return RequestResponse<QuizAttemptDto>.Ok(attempt, "Attempt retrieved successfully.");
        }
    }
}
