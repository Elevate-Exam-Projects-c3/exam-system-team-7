using exam_system.Features.Shared;
using MediatR;
using exam_system.Dtos.Options;

namespace exam_system.Features.Questions.AdminUpdateQuestion.Commands {

    public record UpdateQuestionCommand (Guid QuestionId ,String Text,string? Explanation,int OrderIndex)  : 
        IRequest<RequestResponse<Guid>> {
    }
}
