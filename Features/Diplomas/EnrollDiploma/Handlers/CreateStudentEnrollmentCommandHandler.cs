using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.EnrollDiploma.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.EnrollDiploma.Handlers
{
    public class CreateStudentEnrollmentCommandHandler: IRequestHandler<CreateStudentEnrollmentCommand,RequestResponse<bool>>
    {
        private readonly IGenericRepository<StudentEnrollment> _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudentEnrollmentCommandHandler(IGenericRepository<StudentEnrollment> enrollmentRepository,
                                                     IUnitOfWork unitOfWork)
        {
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse<bool>> Handle(
            CreateStudentEnrollmentCommand request,
            CancellationToken cancellationToken)
        {
            var enrollment = new StudentEnrollment
            {
                Id = Guid.NewGuid(),
                StudentId = request.StudentId,
                DiplomaId = request.DiplomaId,
                EnrolledAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _enrollmentRepository.AddAsync(enrollment);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (affectedRows <= 0)
            {
                return RequestResponse<bool>.Fail("Failed to enroll in diploma.", 500);
            }

            return RequestResponse<bool>.Created(true ,"Enrollment created successfully.");
        }
    }
}
