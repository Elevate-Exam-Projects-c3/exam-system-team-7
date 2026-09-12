using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.CommonQueries.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Diplomas.CommonQueries.Handler
{
    public class HasActiveEnrollmentsQueryHandler: IRequestHandler<HasActiveEnrollmentsQuery, bool>
    {
        private readonly IGenericRepository<StudentEnrollment> _enrollmentRepository;

        public HasActiveEnrollmentsQueryHandler(IGenericRepository<StudentEnrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<bool> Handle(HasActiveEnrollmentsQuery request,
            CancellationToken cancellationToken)
        {
            return await _enrollmentRepository.Get(x => x.DiplomaId == request.DiplomaId && !x.IsDeleted)
                                              .AnyAsync(cancellationToken);
        }
    }
}
