using MediatR;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Common;
using Wordle.Domain.DailyWords;

namespace Wordle.Application.DailyWords.Commands.Delete;

public class DeletePlannedWordCommandHandler : IRequestHandler<DeletePlannedWordCommand>
{
    private readonly IDailyWordRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePlannedWordCommandHandler(IDailyWordRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeletePlannedWordCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteByDateAsync(request.Date);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
