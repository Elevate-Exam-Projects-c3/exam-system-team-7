using exam_system.Common.Enums;
using exam_system.Dtos.Attempts;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Analytics.SearchAttempts.Queries
{
    public record SearchAttemptQuery(Guid? QuizId, Guid? StudentId, AttemptStatus? Status, bool SortDesc, int pageIndex, int PageSize) : IRequest<RequestResponse<PaginatedResult<AttemptSummaryDto>>>;
    
}
