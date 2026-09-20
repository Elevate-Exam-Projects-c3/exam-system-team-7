using exam_system.Common.Enums;
using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Quizzes.AdminUnpublishQuiz.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminUnpublishQuiz.Handlers
{
    public class GetInProgressAttemptsCountQueryHandler :IRequestHandler<HasInProgressAttemptsQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<QuizAttempt> _attemptRepository;

        public GetInProgressAttemptsCountQueryHandler(IGenericRepository<QuizAttempt> attemptRepository)
        {
            _attemptRepository = attemptRepository;
        }

        public async Task<RequestResponse<bool>> Handle(HasInProgressAttemptsQuery request, CancellationToken cancellationToken)
        {
            var hasInprogressAttempt = await _attemptRepository.AnyAsync(a => a.QuizId == request.QuizId && a.Status == AttemptStatus.InProgress && !a.IsDeleted);
            if(hasInprogressAttempt)
            return RequestResponse<bool>.Fail("There is Inprogress Attempt");

            return RequestResponse<bool>.Ok(hasInprogressAttempt);
        }
    }
}
