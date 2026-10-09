using exam_system.Dtos.Options;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.AdminAddOptions.Commands {
    public record CreateOptionsCommand( Guid questionId,
       List<CreateOptionDto> Options) : IRequest<RequestResponse<bool>> {

    }
}
