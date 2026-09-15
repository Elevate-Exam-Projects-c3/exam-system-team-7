using exam_system.Dtos.Diploma.GetDiploma;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.GetDiplomaDetail.Queries
{
    public record GetDiplomaByIdQuery(
        Guid Id
    ) : IRequest<RequestResponse<GetDiplomaByIdResponse>>;
}
