using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.AdminAddOptions.Commands {
    public record CreateOptionsCommand(
        Guid QuestionId, string OptionText, bool IsCorrect ) : IRequest<RequestResponse<Guid>> {

    }
}
