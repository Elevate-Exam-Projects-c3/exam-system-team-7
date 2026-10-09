using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.AdminDeleteOptions.Commands {
    public record DeleteOptionCommand(Guid questionId, Guid optionId) : IRequest<RequestResponse<bool>>{
    }
}
