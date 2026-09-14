using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.EnrollDiploma.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class CheckStudentEnrollmentExistsQueryHandler: IRequestHandler<CheckStudentEnrollmentExistsQuery, bool>
    {
        private readonly IGenericRepository<StudentEnrollment> _enrollmentRepository;

        public CheckStudentEnrollmentExistsQueryHandler(IGenericRepository<StudentEnrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<bool> Handle(CheckStudentEnrollmentExistsQuery request, CancellationToken cancellationToken)
        {
            return await _enrollmentRepository
                .Get(x => x.StudentId == request.StudentId && x.DiplomaId == request.DiplomaId)
                .AnyAsync(cancellationToken);
        }
    }
}
