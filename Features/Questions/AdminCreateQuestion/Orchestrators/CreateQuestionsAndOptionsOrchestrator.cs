using exam_system.Features.Shared;
using exam_system.ViewModels.Questions;
using MediatR;

namespace exam_system.Features.Questions.AdminCreateQuestion.Orchestrators {
    public record CreateQuestionsAndOptionsOrchestrator(
     Guid quizId,
     string Text,
     string? Explanation,
     int OrderIndex,
     List<CreateOptionViewModel> Options) : IRequest<RequestResponse<bool>>;
}
