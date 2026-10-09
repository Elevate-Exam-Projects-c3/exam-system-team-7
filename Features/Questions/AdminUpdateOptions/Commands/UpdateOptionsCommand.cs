using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.AdminUpdateOptions.Commands {
    public record UpdateOptionsCommand(
        Guid questionId, Guid optionId,string OptionText, bool IsCorrect ) : IRequest<RequestResponse<Guid>> {

    }
}
