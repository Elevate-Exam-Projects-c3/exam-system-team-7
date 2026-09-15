using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.GetDiplomaDetail.Queries {
    public record CheckIfDiplomaExistQuery(Guid DiplomaId) : IRequest<RequestResponse<bool>> { 

    }
}
