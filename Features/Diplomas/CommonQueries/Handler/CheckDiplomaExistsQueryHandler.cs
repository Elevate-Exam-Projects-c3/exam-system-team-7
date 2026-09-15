using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.CommonQueries.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.CommonQueries.Handler
{
    public class CheckDiplomaExistsQueryHandler : IRequestHandler<CheckDiplomaExistsQuery, bool>
    {
        private readonly IGenericRepository<Diploma> _diplomaRepository;

        public CheckDiplomaExistsQueryHandler(IGenericRepository<Diploma> diplomaRepository)
        {
            _diplomaRepository = diplomaRepository;
        }

        public async Task<bool> Handle(
            CheckDiplomaExistsQuery request,
            CancellationToken cancellationToken)
        {
            return await _diplomaRepository
                .Get(x =>x.Id == request.DiplomaId && !x.IsDeleted).AnyAsync(cancellationToken);
        }
    }
}
