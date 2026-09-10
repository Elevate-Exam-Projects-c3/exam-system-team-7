using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.AdminDeleteDiploma.Commands
{
    public record DeleteDiplomaCommand(
        Guid Id
    ) : IRequest<RequestResponse<bool>>;
}
