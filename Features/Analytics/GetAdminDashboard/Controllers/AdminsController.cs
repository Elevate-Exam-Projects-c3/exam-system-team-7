using exam_system.Features.Analytics.GetAdminDashboard.Queries;
using exam_system.ViewModels.Admin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Analytics.GetAdminDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<AdminDashboardViewModel>> GetDashboard()
        {
            
                var result = await mediator.Send(new AdminDashboardOrchestrator());
                var adminDashBoardViewModel = new AdminDashboardViewModel
                {
                    AveragePassRate = result.AveragePassRate,
                    TotalActiveUsers = result.TotalActiveUsers,
                    TotalAttembts = result.TotalAttembts,
                    TotalNumberOfDiplomas = result.TotalNumberOfDiplomas,
                    TotalNumberOfQuizes = result.TotalNumberOfQuizes,
                    TotalRegistredUsers = result.TotalRegistredUsers

                };
                return Ok(adminDashBoardViewModel);
            
            }
          
    }
}
