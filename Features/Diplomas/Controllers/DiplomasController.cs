using exam_system.Common.Enums;
using exam_system.Dtos.Diploma.CreateDiploma;
using exam_system.Dtos.Diploma.GetDiploma;
using exam_system.Dtos.Diploma.UpdateDiploma;
using exam_system.Features.Diplomas.AdminCreateDiploma.Commands;
using exam_system.Features.Diplomas.AdminDeleteDiploma.Orchestrators;
using exam_system.Features.Diplomas.AdminUpdateDiploma.Commands;
using exam_system.Features.Diplomas.GetDiplomaDetail.Queries;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Diplomas.Controllers
{
    [ApiController]
    [Route("api/admin/diplomas")]
    public class DiplomasController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly DeleteDiplomaOrchestrator _deleteDiplomaOrchestrator;

        public DiplomasController(
            IMediator mediator,
            DeleteDiplomaOrchestrator deleteDiplomaOrchestrator)
        {
            _mediator = mediator;
            _deleteDiplomaOrchestrator = deleteDiplomaOrchestrator;
        }

        [HttpPost]
        //[Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Create([FromBody] CreateDiplomaDto request,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new CreateDiplomaCommand(request),
                cancellationToken);

            var response = EndpointResponse<bool>.FromResult(result);

            return StatusCode(
                response.StatusCode,
                response);
        }


        [HttpPut("{id:guid}")]
        //[Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Update(Guid id,[FromBody] UpdateDiplomaDto request,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new UpdateDiplomaCommand(id, request),
                cancellationToken);

            var response = EndpointResponse<bool>.FromResult(result);

            return StatusCode(
                response.StatusCode,
                response);
        }

        [HttpDelete("{id:guid}")]
        //[Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _deleteDiplomaOrchestrator.ExecuteAsync(id,cancellationToken);

            var response = EndpointResponse<bool>.FromResult(result);

            return StatusCode(
                response.StatusCode,
                response);
        }

        [HttpGet("{id:guid}")]
        //[Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetById(Guid id,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send( new GetDiplomaByIdQuery(id), cancellationToken);

            var response = EndpointResponse<GetDiplomaByIdResponse>.FromResult(result);

            return StatusCode( response.StatusCode, response);
        }
    }
}
