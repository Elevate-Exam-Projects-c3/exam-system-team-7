using exam_system.Dtos.Attempts;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Analytics.GetAdminAttemptDetail.Queries
{
    public record GetAdminAttemptDetailQuery(Guid AttemptId) :IRequest<RequestResponse<AttemptDetailDto>>;


}
