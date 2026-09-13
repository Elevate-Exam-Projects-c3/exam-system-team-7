using exam_system.Dtos.Diploma.BrowseDiplomas;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.BrowseDiplomas.Queries
{
    public record BrowseDiplomasQuery(
        BrowseDiplomasDto RequestDto
    ) : IRequest<RequestResponse<PaginatedResult<BrowseDiplomaItemDto>>>;
}
