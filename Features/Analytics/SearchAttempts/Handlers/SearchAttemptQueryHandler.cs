using exam_system.Domain.Entities.Attempts;
using exam_system.Dtos.Attempts;
using exam_system.Features.Analytics.SearchAttempts.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Analytics.SearchAttempts.Handlers
{
    public class SearchAttemptQueryHandler :IRequestHandler<SearchAttemptQuery , RequestResponse<PaginatedResult<AttemptSummaryDto>>>
    {
        private readonly IGenericRepository<QuizAttempt> _attemptRepository;

        public SearchAttemptQueryHandler(IGenericRepository<QuizAttempt> attemptRepository )
        {
            _attemptRepository = attemptRepository;
        }

        public async Task<RequestResponse<PaginatedResult<AttemptSummaryDto>>> Handle(SearchAttemptQuery request, CancellationToken cancellationToken)
        {
            var attempts = _attemptRepository.GetAll().Where(a=>!a.IsDeleted);
            
            if(request.QuizId.HasValue)
            {
                attempts = attempts.Where(a => a.QuizId == request.QuizId);
            }
            if(request.StudentId.HasValue)
            {
                attempts = attempts.Where(a => a.StudentId == request.StudentId);
            }
            if(request.Status.HasValue)
            {
                attempts = attempts.Where(a => a.Status == request.Status);
            }

            attempts = request.SortDesc 
                ? attempts.OrderByDescending(a => a.SubmittedAt) 
                : attempts.OrderBy(a => a.SubmittedAt);

            var totalCount = await attempts.CountAsync(cancellationToken);

            var items = await attempts
                .Skip((request.pageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(a => new AttemptSummaryDto
                {
                    AttemptId = a.Id,
                    QuizTitle = a.Quiz.Title,
                    StudentName = a.Student.User.FullName,
                    Status = a.Status.ToString(),
                    Score = a.Score,
                    SubmittedAt = a.SubmittedAt,
                    StartTime = a.StartTime
                }).ToListAsync(cancellationToken);

            var result = PaginatedResult<AttemptSummaryDto>.Create(items, totalCount, request.pageIndex, request.PageSize);

            return RequestResponse<PaginatedResult<AttemptSummaryDto>>.Ok(result);

        }
    }
}
