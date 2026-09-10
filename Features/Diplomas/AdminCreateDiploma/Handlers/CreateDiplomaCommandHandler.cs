using exam_system.Domain.Entities.Diplomas;
using exam_system.Dtos.Diploma.CreateDiploma;
using exam_system.Features.Diplomas.AdminCreateDiploma.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Diplomas.AdminCreateDiploma.Handlers
{

    public class CreateDiplomaCommandHandler
    : IRequestHandler<
        CreateDiplomaCommand,
        RequestResponse<bool>>
    {
        private readonly IGenericRepository<Diploma> _diplomaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDiplomaCommandHandler(IGenericRepository<Diploma> diplomaRepository,IUnitOfWork unitOfWork)
        {
            _diplomaRepository = diplomaRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<RequestResponse<bool>> Handle( CreateDiplomaCommand request, CancellationToken cancellationToken)
        {

            var diploma = new Diploma
            {
                Id = Guid.NewGuid(),
                Title = request.RequestDto.Title.Trim(),
                Description = request.RequestDto.Description?.Trim(),

                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _diplomaRepository.AddAsync(diploma);
            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (affectedRows <= 0)
            {
                return RequestResponse<bool>.Fail("Failed to create diploma.",500);
            }

            return RequestResponse<bool>.Created(true);
        }
    }
}
