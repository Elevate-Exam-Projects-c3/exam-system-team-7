using exam_system.Domain.Entities.Identity;
using exam_system.Features.Analytics.GetAdminDashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Analytics.GetAdminDashboard.Handlers
{
    public class GetAllRegistredUsersNumbersQueryHandler : IRequestHandler<GetAllRegisteredUserNumberQuery, int>
    {

        
        public GetAllRegistredUsersNumbersQueryHandler()
        {
            
        }
        public async Task<int> Handle(GetAllRegisteredUserNumberQuery request, CancellationToken cancellationToken)
        {
          
            return 0;
        }
    }
}
