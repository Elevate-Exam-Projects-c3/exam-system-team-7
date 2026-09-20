using MediatR;

namespace exam_system.Features.Diplomas.GetStudentDashboard.Queries
{
    public record OverAllScoreQuery(Guid UserId) : IRequest<double>;
    
}
