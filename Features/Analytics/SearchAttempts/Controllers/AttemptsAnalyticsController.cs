using exam_system.Common.Enums;
using exam_system.Dtos.Attempts;
using exam_system.Features.Analytics.SearchAttempts.Queries;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Analytics.SearchAttempts.Controllers
{
    [ApiController]
    public class AttemptsAnalyticsController : Controller
    {
        private readonly IMediator _mediator;

        
        public AttemptsAnalyticsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("/api/admin/attempts")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<RequestResponse<PaginatedResult<AttemptSummaryDto>>>> SearchAttempts([FromQuery] Guid? quizId, [FromQuery] Guid? studentId,
                                                                                                                [FromQuery] AttemptStatus? status, [FromQuery] bool sortDesc = true,
                                                                                                                [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new SearchAttemptQuery(quizId, studentId, status, sortDesc, pageIndex, pageSize));
            if(result == null)
                return RequestResponse<PaginatedResult<AttemptSummaryDto>>.Fail("No attempts found.");
            return RequestResponse<PaginatedResult<AttemptSummaryDto>>.Ok(result.Data);
        }
    }
}
