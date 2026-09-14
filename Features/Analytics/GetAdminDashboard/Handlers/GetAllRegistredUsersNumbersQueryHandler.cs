using exam_system.Domain.Entities.Identity;
using exam_system.Features.Analytics.GetAdminDashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Analytics.GetAdminDashboard.Handlers
{
    public class GetAllRegistredUsersNumbersQueryHandler : IRequestHandler<GetAllRegisteredUserNumberQuery, int>
    {

        private readonly UserManager<AppUser> _userManger;
        public GetAllRegistredUsersNumbersQueryHandler(UserManager<AppUser> userManager)
        {
            _userManger = userManager;
        }
        public async Task<int> Handle(GetAllRegisteredUserNumberQuery request, CancellationToken cancellationToken)
        {
          int numberOfRegistredUsers= await _userManger.Users.CountAsync();
            return numberOfRegistredUsers;
        }
    }
}
