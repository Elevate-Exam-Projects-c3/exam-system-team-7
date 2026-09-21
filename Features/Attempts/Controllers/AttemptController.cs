using exam_system.Dtos.Attempts;
using exam_system.Features.Attempts.GetAttemptHistory.Queries;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Attempts.Controllers
{
    public class AttemptController : Controller
    {
        private readonly IMediator _mediator;

        public AttemptController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("history")]
            public async Task<ActionResult<EndpointResponse<PaginatedResult<AttemptHistoryRecordsDto>>>> GetAttemptHistory(
                    [FromQuery] int pageIndex, CancellationToken cancellationToken) 
            {
                var studentId = Guid.Parse("AAAAAAAA-1111-1111-1111-AAAAAAAAAAAA"); 
                var result = await _mediator.Send(
                    new GetAttemptHistoryQuery(studentId, new RequestPageDetailsDto { PageIndex = pageIndex }), cancellationToken); 

                return StatusCode(result.StatusCode, EndpointResponse<PaginatedResult<AttemptHistoryRecordsDto>>.FromResult(result)); 
            }

        
    }
}
