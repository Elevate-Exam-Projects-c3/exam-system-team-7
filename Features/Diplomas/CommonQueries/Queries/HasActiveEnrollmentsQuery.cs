using MediatR;

namespace exam_system.Features.Diplomas.CommonQueries.Queries
{
    public record HasActiveEnrollmentsQuery( Guid DiplomaId) : IRequest<bool>;
}
