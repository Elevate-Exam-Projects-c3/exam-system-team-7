using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Analytics.GetAdminDashboard.Queries
{
    public record AdminDashboardOrchestrator : IRequest<AdminDashoardDto>;
    
}
