using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminUpdateDiploma.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.AdminUpdateDiploma.Handlers
{
    public class UpdateDiplomaCommandHandler
        : IRequestHandler<UpdateDiplomaCommand, RequestResponse<bool>>
    {
        private readonly IGenericRepository<Diploma> _diplomaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDiplomaCommandHandler(
            IGenericRepository<Diploma> diplomaRepository,
            IUnitOfWork unitOfWork)
        {
            _diplomaRepository = diplomaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse<bool>> Handle(
            UpdateDiplomaCommand request,
            CancellationToken cancellationToken)
        {
            var exists = await _diplomaRepository
                .Get(x => x.Id == request.Id && !x.IsDeleted).AnyAsync(cancellationToken);

            if (!exists)
            {
                return RequestResponse<bool>.Fail( "Diploma not found.",404);
            }

            var diploma = new Diploma
            {
                Id = request.Id,
                Title = request.RequestDto.Title.Trim(),
                Description = request.RequestDto.Description?.Trim(),
                UpdatedAt = DateTime.UtcNow
            };

            _diplomaRepository.SaveInclude(
                diploma,
                nameof(Diploma.Title),
                nameof(Diploma.Description),
                nameof(Diploma.UpdatedAt)
            );

            var affectedRows =
                await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (affectedRows <= 0)
            {
                return RequestResponse<bool>.Fail( "Failed to update diploma.",500);
            }

            return RequestResponse<bool>.Ok( true, "Diploma updated successfully.");
        }
    }
}
