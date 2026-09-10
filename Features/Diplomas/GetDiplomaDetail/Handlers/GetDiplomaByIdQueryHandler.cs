using exam_system.Domain.Entities.Diplomas;
using exam_system.Dtos.Diploma.GetDiploma;
using exam_system.Features.Diplomas.GetDiplomaDetail.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.GetDiplomaDetail.Handlers
{
    public class GetDiplomaByIdQueryHandler: IRequestHandler<GetDiplomaByIdQuery,RequestResponse<GetDiplomaByIdResponse>>
    {
        private readonly IGenericRepository<Diploma> _diplomaRepository;

        public GetDiplomaByIdQueryHandler(IGenericRepository<Diploma> diplomaRepository)
        {
            _diplomaRepository = diplomaRepository;
        }

        public async Task<RequestResponse<GetDiplomaByIdResponse>> Handle(
            GetDiplomaByIdQuery request,
            CancellationToken cancellationToken)
        {
            var diploma = await _diplomaRepository.Get(x => x.Id == request.Id && !x.IsDeleted).AsNoTracking()
                .Select(x => new GetDiplomaByIdResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (diploma is null)
            {
                return RequestResponse<GetDiplomaByIdResponse>.Fail("Diploma not found.",404);
            }

            return RequestResponse<GetDiplomaByIdResponse>.Ok( diploma);
        }
    }
}
