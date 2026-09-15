using exam_system.Features.Diplomas.CommonQueries.Queries;
using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Diplomas.EnrollDiploma.Orchestrators;
using exam_system.Features.Diplomas.EnrollDiploma.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class EnrollDiplomaOrchestratorHandler : IRequestHandler<EnrollDiplomaOrchestrator, RequestResponse<bool>>
    {
        private readonly IMediator _mediator;

        public EnrollDiplomaOrchestratorHandler( IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResponse<bool>> Handle(EnrollDiplomaOrchestrator request, CancellationToken cancellationToken)
        {
            var diplomaExists = await _mediator.Send(new CheckDiplomaExistsQuery(request.DiplomaId), cancellationToken);

            if (!diplomaExists)
            {
                return RequestResponse<bool>.Fail("Diploma not found.", 404);
            }

            var alreadyEnrolled = await _mediator.Send(new CheckStudentEnrollmentExistsQuery( request.StudentId, request.DiplomaId), cancellationToken);

            if (alreadyEnrolled)
            {
                return RequestResponse<bool>.Fail("Student is already enrolled in this diploma.", 409);
            }

            return await _mediator.Send(
                new CreateStudentEnrollmentCommand( request.StudentId, request.DiplomaId),cancellationToken);
        }
    }
}
