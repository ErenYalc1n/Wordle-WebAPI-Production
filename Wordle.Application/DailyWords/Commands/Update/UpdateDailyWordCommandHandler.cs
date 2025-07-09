using MediatR;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Common;
using Wordle.Domain.DailyWords;

namespace Wordle.Application.DailyWords.Commands.Update;

public class UpdateDailyWordCommandHandler : IRequestHandler<UpdateDailyWordCommand>
{
    private readonly IDailyWordRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDailyWordCommandHandler(IDailyWordRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateDailyWordCommand request, CancellationToken cancellationToken)
    {
        var oldDate = request.DailyWord.OldDate.ToDateTime(TimeOnly.MinValue);
        var newDate = request.DailyWord.NewDate.ToDateTime(TimeOnly.MinValue);

        var existing = await _repository.GetTodayWordAsync(request.DailyWord.OldDate);
        if (existing is null)
            throw new InvalidOperationException("Güncellenecek kelime bulunamadı.");

        var now = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(3));
        if (request.DailyWord.OldDate <= now)
            throw new InvalidOperationException("Geçmiş veya bugünkü kelimeler güncellenemez.");

        if (request.DailyWord.OldDate != request.DailyWord.NewDate)
        {
            var isDateTaken = await _repository.IsDateTakenAsync(newDate);
            if (isDateTaken)
                throw new InvalidOperationException("Yeni tarih zaten başka bir kelime tarafından kullanılıyor.");
        }

        existing.Word = request.DailyWord.Word;
        existing.Date = newDate;

        await _repository.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
