using exam_system.Dtos;
using exam_system.Features.Diplomas.GetStudentDashboard.Queries;
using MediatR;

namespace exam_system.Features.Diplomas.GetStudentDashboard.Orchestrators
{
    public class StudentDashBoardOrchestratorHandler : IRequestHandler<StudentsDashBoardOrchestrator, StudentsDashboardDto>
    {
        private readonly IMediator _mediator;
        public StudentDashBoardOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<StudentsDashboardDto> Handle(StudentsDashBoardOrchestrator request, CancellationToken cancellationToken)
        {
           double overAllScore= await _mediator.Send(new OverAllScoreQuery(request.userId));
           bool isPassed = await _mediator.Send(new PassedOutcomeQuery(request.userId));
            return new StudentsDashboardDto()
            {
                OverAllScoure = overAllScore,
                PassedStatus = isPassed,
                Message = "The Student Status"
            };
        }
    }
}
