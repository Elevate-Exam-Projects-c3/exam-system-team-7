using MediatR;

namespace exam_system.Features.Diplomas.GetStudentDashboard.Queries
{
    public record PassedOutcomeQuery(Guid UserId) : IRequest<bool>;
    
}
