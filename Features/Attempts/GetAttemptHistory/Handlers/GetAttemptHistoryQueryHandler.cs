using exam_system.Domain.Entities.Attempts;
using exam_system.Dtos.Attempts;
using exam_system.Features.Attempts.GetAttemptHistory.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.GetAttemptHistory.Handlers
{
    public class GetAttemptHistoryQueryHandler : IRequestHandler<GetAttemptHistoryQuery, RequestResponse<PaginatedResult<AttemptHistoryRecordsDto>>>
    {
        private const int PageSize = 10;
        private readonly IGenericRepository<QuizAttempt> _quizAttemptRepository;

        public GetAttemptHistoryQueryHandler(IGenericRepository<QuizAttempt> quizAttemptRepository)
        {
            _quizAttemptRepository = quizAttemptRepository;
        }
        public async Task<RequestResponse<PaginatedResult<AttemptHistoryRecordsDto>>> Handle(GetAttemptHistoryQuery request, CancellationToken cancellationToken)
        {
            var attempts = _quizAttemptRepository.GetAll().AsNoTracking()
                .Where(a => a.StudentId == request.studentId && !a.IsDeleted);
            var pageIndex = request.pageDetails.PageIndex;

            var items =  attempts
                .OrderByDescending(a => a.SubmittedAt ?? a.StartTime)
                .Skip((pageIndex - 1) * PageSize)
                .Take(PageSize)
                .Select(a => new AttemptHistoryRecordsDto
                {
                    AttemptId = a.Id,
                    QuizTitle = a.Quiz.Title,
                    Status = a.Status.ToString(),
                    Score = a.Score,
                    SubmittedAt = a.SubmittedAt,
                    StartedAt = a.StartTime
                }).ToListAsync(cancellationToken);
            
            var totalCount = await attempts.CountAsync(cancellationToken);

            var result = PaginatedResult<AttemptHistoryRecordsDto>.Create(await items, totalCount, pageIndex, PageSize);
            if(result.Items.Count == 0)
            {
                return RequestResponse<PaginatedResult<AttemptHistoryRecordsDto>>.Fail("No attempt history found for the specified student.");
            }
            return RequestResponse<PaginatedResult<AttemptHistoryRecordsDto>>.Ok(result);
        }
    }
}
