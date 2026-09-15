using MediatR;

namespace exam_system.Features.Analytics.GetAdminDashboard.Queries
{
    public record GetTotalAttemptsQuery : IRequest<int>;
    
}
