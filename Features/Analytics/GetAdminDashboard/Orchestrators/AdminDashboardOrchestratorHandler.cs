using exam_system.Features.Analytics.GetAdminDashboard.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Analytics.GetAdminDashboard.Orchestrators
{
    public class AdminDashboardOrchestratorHandler : IRequestHandler<AdminDashboardOrchestrator, AdminDashoardDto>
    {
        private readonly IMediator _mediator;
        public AdminDashboardOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<AdminDashoardDto> Handle(AdminDashboardOrchestrator request, CancellationToken cancellationToken)
        {
           double numberOfRegistredUsers= await _mediator.Send(new GetAllRegisteredUserNumberQuery());
            double numberOfAllPublishedQuizes = await _mediator.Send(new GetAllQuizesNumberQuery());
            double numberOfAllDiplomas = await _mediator.Send(new GetAllDiplomasNubmerQuery());
            double averagePassRate = await _mediator.Send(new GetAveragePassRateQuery());
            double totalNumberOfAttembts = await _mediator.Send(new GetTotalAttemptsQuery());
            return new AdminDashoardDto()
            {
                TotalRegistredUsers = numberOfRegistredUsers,
                TotalNumberOfQuizes = numberOfAllPublishedQuizes,
                TotalNumberOfDiplomas = numberOfAllDiplomas,
                AveragePassRate = averagePassRate,
                TotalAttembts = totalNumberOfAttembts,
                
            };
        }
    }
}
