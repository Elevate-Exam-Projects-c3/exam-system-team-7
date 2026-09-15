using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminDeleteDiploma.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.AdminDeleteDiploma.Handlers
{
    public class DeleteDiplomaCommandHandler
        : IRequestHandler<DeleteDiplomaCommand, RequestResponse<bool>>
    {
        private readonly IGenericRepository<Diploma> _diplomaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDiplomaCommandHandler(
            IGenericRepository<Diploma> diplomaRepository,
            IUnitOfWork unitOfWork)
        {
            _diplomaRepository = diplomaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse<bool>> Handle(
            DeleteDiplomaCommand request,
            CancellationToken cancellationToken)
        {

            var diploma = new Diploma
            {
                Id = request.Id,
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow
            };

            _diplomaRepository.SaveInclude(
                diploma,
                nameof(Diploma.IsDeleted),
                nameof(Diploma.DeletedAt)
            );

            var affectedRows =
                await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (affectedRows <= 0)
            {
                return RequestResponse<bool>.Fail("Failed to delete diploma.", 500);
            }

            return RequestResponse<bool>.Ok( true,"Diploma deleted successfully.");
        }
    }
}
