using exam_system.Dtos.Attempts;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.GetAttemptHistory.Queries
{
    public record GetAttemptHistoryQuery(Guid studentId , RequestPageDetailsDto pageDetails) : IRequest<RequestResponse<PaginatedResult<AttemptHistoryRecordsDto>>>;
   
    
}
