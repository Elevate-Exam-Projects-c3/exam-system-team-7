using exam_system.Features.Diplomas.AdminDeleteDiploma.Commands;
using exam_system.Features.Diplomas.AdminDeleteDiploma.Orchestrators;
using exam_system.Features.Diplomas.CommonQueries.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.AdminDeleteDiploma.Handlers
{

    public class DeleteDiplomaOrchestratorHandler
    : IRequestHandler<
        DeleteDiplomaOrchestrator,
        RequestResponse<bool>>
    {
        private readonly IMediator _mediator;


        public DeleteDiplomaOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResponse<bool>> Handle(DeleteDiplomaOrchestrator request, CancellationToken cancellationToken)
        {
            var diplomaExists = await _mediator.Send(new CheckDiplomaExistsQuery(request.DiplomaId), cancellationToken);

            if (!diplomaExists)
            {
                return RequestResponse<bool>.Fail( "Diploma not found.", 404);
            }

            var hasActiveEnrollments = await _mediator.Send( new HasActiveEnrollmentsQuery(request.DiplomaId), cancellationToken);

            if (hasActiveEnrollments)
            {
                return RequestResponse<bool>.Fail( "Diploma cannot be deleted because it has active enrollments.",  409);
            }

            return await _mediator.Send( new DeleteDiplomaCommand(request.DiplomaId), cancellationToken);
        }
    }
 }

