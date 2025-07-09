using MediatR;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Common;
using Wordle.Domain.DailyWords;

namespace Wordle.Application.DailyWords.Commands.Add;

public class AddDailyWordCommandHandler : IRequestHandler<AddDailyWordCommand, Guid>
{
    private readonly IDailyWordRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AddDailyWordCommandHandler(IDailyWordRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddDailyWordCommand request, CancellationToken cancellationToken)
    {
        var date = request.DailyWord.Date.ToDateTime(TimeOnly.MinValue);

        var isTaken = await _repository.IsDateTakenAsync(date);
        if (isTaken)
            throw new InvalidOperationException("Bu tarihe ait bir kelime zaten var.");

        var word = new DailyWord
        {
            Id = Guid.NewGuid(),
            Word = request.DailyWord.Word,
            Date = date
        };

        await _repository.AddAsync(word);       
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        

        return word.Id;
    }
}
