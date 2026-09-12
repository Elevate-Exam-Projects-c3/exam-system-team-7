using MediatR;

namespace exam_system.Features.Diplomas.CommonQueries.Queries
{
    public record CheckDiplomaExistsQuery(
        Guid DiplomaId
    ) : IRequest<bool>;
}
