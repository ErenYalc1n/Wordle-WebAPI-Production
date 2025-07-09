using Microsoft.EntityFrameworkCore;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.DailyWords;
using Wordle.Infrastructure.Data;

namespace Wordle.Infrastructure.Repositories;

public class EfDailyWordRepository : IDailyWordRepository
{
    private readonly WordleDbContext _context;

    public EfDailyWordRepository(WordleDbContext context)
    {
        _context = context;
    }

    public async Task<DailyWord?> GetTodayWordAsync(DateOnly date)
    {
        return await _context.DailyWords
            .FirstOrDefaultAsync(x => x.Date.Date == date.ToDateTime(TimeOnly.MinValue).Date);
    }

    public Task AddAsync(DailyWord word)
    {
        _context.DailyWords.Add(word);
        return Task.CompletedTask;
    }

    public async Task<List<DailyWord>> SearchAsync(string keyword, bool isPast, int page, int pageSize)
    {
        var query = _context.DailyWords
            .Where(w => w.Word.Contains(keyword));

        if (isPast)
            query = query.Where(w => w.Date < DateTime.UtcNow.Date);
        else
            query = query.Where(w => w.Date > DateTime.UtcNow.Date);

        return await query
            .OrderByDescending(w => w.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> IsDateTakenAsync(DateTime date)
    {
        return await _context.DailyWords.AnyAsync(w => w.Date.Date == date.Date);
    }

    public async Task DeleteAsync(Guid id)
    {
        var word = await _context.DailyWords.FindAsync(id);
        if (word is not null)
        {
            _context.DailyWords.Remove(word);
           
        }
    }

    public Task UpdateAsync(DailyWord updatedWord)
    {
        _context.DailyWords.Update(updatedWord);
        return Task.CompletedTask;
    }

    public async Task<List<DailyWord>> GetPastWordsAsync(int page, int pageSize)
    {
        var today = DateTime.UtcNow.AddHours(3).Date;

        return await _context.DailyWords
            .Where(w => w.Date < today)
            .OrderByDescending(w => w.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<DailyWord>> GetPlannedWordsAsync(int page, int pageSize)
    {
        return await _context.DailyWords
            .Where(w => w.Date > DateTime.UtcNow.Date)
            .OrderBy(w => w.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountPlannedAsync()
    {
        return await _context.DailyWords
            .CountAsync(w => w.Date > DateTime.UtcNow.AddHours(3).Date);
    }

    public async Task<int> CountPastAsync()
    {
        var today = DateTime.UtcNow.AddHours(3).Date;

        return await _context.DailyWords
            .CountAsync(w => w.Date < today);
    }

    public async Task DeleteByDateAsync(DateOnly date)
    {
        var word = await _context.DailyWords
            .FirstOrDefaultAsync(w => w.Date == date.ToDateTime(TimeOnly.MinValue));

        if (word is null)
            throw new NotFoundException("Bu tarihte planlı bir kelime bulunamadı.");

        if (word.Date <= DateTime.UtcNow.AddHours(3).Date)
            throw new InvalidOperationException("Geçmiş veya bugünkü kelime silinemez.");

        _context.DailyWords.Remove(word);
    }

    public async Task<DailyWord?> GetByDateAsync(DateOnly date)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);
        return await _context.DailyWords
            .FirstOrDefaultAsync(w => w.Date.Date == dateTime.Date);
    }

    public async Task<DailyWord?> GetByWordAsync(string word)
    {
        return await _context.DailyWords
            .FirstOrDefaultAsync(w => w.Word.ToLower() == word.ToLower());
    }
}
