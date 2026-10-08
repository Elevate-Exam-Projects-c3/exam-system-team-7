using exam_system.Common.Enums;
using exam_system.Dtos.Attempts;
using exam_system.Features.Analytics.GetAdminAttemptDetail.Queries;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Analytics.GetAdminAttemptDetail.Controllers
{
    [ApiController]
    public class AdminAttemptController : Controller
    {
        private readonly IMediator _mediator;

        public AdminAttemptController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("/api/admin/attempts/{attemptId:guid}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<EndpointResponse<AttemptDetailDto>>> GetDetail(Guid attemptId,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAdminAttemptDetailQuery(attemptId), cancellationToken);
            return EndpointResponse<AttemptDetailDto>.FromResult(result);
        }
    }
}
