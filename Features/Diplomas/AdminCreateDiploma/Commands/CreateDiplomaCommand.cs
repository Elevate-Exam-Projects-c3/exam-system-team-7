using exam_system.Dtos.Diploma.CreateDiploma;
using exam_system.Features.Shared;
using MediatR;


namespace exam_system.Features.Diplomas.AdminCreateDiploma.Commands
{

    public record CreateDiplomaCommand(
        CreateDiplomaDto RequestDto
    ) : IRequest<RequestResponse<bool>>;
}
