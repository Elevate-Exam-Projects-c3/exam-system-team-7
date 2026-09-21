using exam_system.Features.Diplomas.GetStudentDashboard.Orchestrators;
using exam_system.Features.Diplomas.GetStudentDashboard.Queries;
using exam_system.ViewModels.Students;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.GetStudentDashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsDashboardController:ControllerBase
    {
        private readonly IMediator _mediator;
        public StudentsDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<ActionResult<StudentsDashboardViewModel>> GetOverAllScore(Guid StudentId)
        {
          var studentDashBoard = await _mediator.Send(new StudentsDashBoardOrchestrator(StudentId));
            StudentsDashboardViewModel dashBoard = new StudentsDashboardViewModel()
            {
                Message = studentDashBoard.Message,
                OverAllScoure = studentDashBoard.OverAllScoure,
                PassedStatus = studentDashBoard.PassedStatus,
                StatusCode = 200
            };

            return Ok(dashBoard);
        }
    }
}
