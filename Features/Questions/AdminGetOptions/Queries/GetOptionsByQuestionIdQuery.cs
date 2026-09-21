using exam_system.Dtos.Options;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.AdminGetOptions.Queries {
    public record GetOptionsByQuestionIdQuery(Guid questionId)  :IRequest<RequestResponse<List<OptionDto>>>{
    }
}
