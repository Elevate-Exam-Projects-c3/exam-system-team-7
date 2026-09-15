using exam_system.Features.Shared;
using exam_system.ViewModels.Options;
using MediatR;

namespace exam_system.Features.Questions.AdminUpdateQuestion.Orchestrators {
   public record UpdateQuestionOptionsOrchestrator(
        Guid QuestionId,
        string Text,
        string? Explanation,
        int OrderIndex,
        List<UpdateOptionViewModel> Options) : IRequest<RequestResponse<bool>>;

}
