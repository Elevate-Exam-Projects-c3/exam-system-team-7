using exam_system.Dtos.Diploma.UpdateDiploma;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.AdminUpdateDiploma.Commands;

public record UpdateDiplomaCommand(
    Guid Id,
    UpdateDiplomaDto RequestDto
) : IRequest<RequestResponse<bool>>;