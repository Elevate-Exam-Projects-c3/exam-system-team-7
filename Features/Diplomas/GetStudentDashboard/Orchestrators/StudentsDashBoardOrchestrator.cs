using exam_system.Dtos;
using MediatR;

namespace exam_system.Features.Diplomas.GetStudentDashboard.Orchestrators
{
    public record StudentsDashBoardOrchestrator(Guid userId) : IRequest<StudentsDashboardDto>;
    
}
