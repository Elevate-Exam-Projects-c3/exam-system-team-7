using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminDeleteDiploma.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.AdminDeleteDiploma.Orchestrators
{
    public class DeleteDiplomaOrchestrator
    {
        private readonly IGenericRepository<Diploma> _diplomaRepository;
        private readonly IGenericRepository<StudentEnrollment> _enrollmentRepository;
        private readonly IMediator _mediator;

        public DeleteDiplomaOrchestrator(
            IGenericRepository<Diploma> diplomaRepository,
            IGenericRepository<StudentEnrollment> enrollmentRepository,
            IMediator mediator)
        {
            _diplomaRepository = diplomaRepository;
            _enrollmentRepository = enrollmentRepository;
            _mediator = mediator;
        }

        public async Task<RequestResponse<bool>> ExecuteAsync(
            Guid diplomaId,
            CancellationToken cancellationToken)
        {
            var diplomaExists = await _diplomaRepository
                .Get(x => x.Id == diplomaId && !x.IsDeleted).AnyAsync(cancellationToken);

            if (!diplomaExists)
            {
                return RequestResponse<bool>.Fail("Diploma not found.",404);
            }

            var hasActiveEnrollments = await _enrollmentRepository
                .Get(x =>x.DiplomaId == diplomaId && !x.IsDeleted).AnyAsync(cancellationToken);

            if (hasActiveEnrollments)
            {
                return RequestResponse<bool>.Fail("Diploma cannot be deleted because it has active enrollments.", 409);
            }

            return await _mediator.Send(new DeleteDiplomaCommand(diplomaId),cancellationToken);
        }
    }
}
