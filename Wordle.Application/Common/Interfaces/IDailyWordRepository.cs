using Wordle.Domain.DailyWords;

namespace Wordle.Application.Common.Interfaces;

public interface IDailyWordRepository
{
    Task<DailyWord?> GetTodayWordAsync(DateOnly date);
    Task<List<DailyWord>> GetPastWordsAsync(int page, int pageSize);
    Task<List<DailyWord>> GetPlannedWordsAsync(int page, int pageSize);
    Task AddAsync(DailyWord word);       
    Task<bool> IsDateTakenAsync(DateTime date);      
    Task UpdateAsync(DailyWord updatedWord);
    Task<int> CountPlannedAsync();
    Task<int> CountPastAsync();
    Task DeleteByDateAsync(DateOnly date);
    Task<DailyWord?> GetByDateAsync(DateOnly date);
    Task<DailyWord?> GetByWordAsync(string word);


}
