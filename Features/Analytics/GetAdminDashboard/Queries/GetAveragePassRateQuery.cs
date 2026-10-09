using MediatR;

namespace exam_system.Features.Analytics.GetAdminDashboard.Queries
{
    public record GetAveragePassRateQuery : IRequest<double>;
    
}
