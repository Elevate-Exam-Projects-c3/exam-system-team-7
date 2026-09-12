using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Diplomas.AdminDeleteDiploma.Commands;
using exam_system.Features.Diplomas.CommonQueries.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.AdminDeleteDiploma.Orchestrators
{
    public record DeleteDiplomaOrchestrator(
        Guid DiplomaId
    ) : IRequest<RequestResponse<bool>>;
}
}
