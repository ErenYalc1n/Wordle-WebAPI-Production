using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wordle.Application.DailyWords.Commands.Add;
using Wordle.Application.DailyWords.Commands.Delete;
using Wordle.Application.DailyWords.Commands.Update;
using Wordle.Application.DailyWords.DTOs;
using Wordle.Application.DailyWords.Queries.GetPastList;
using Wordle.Application.DailyWords.Queries.GetPlannedList;
using Wordle.Application.DailyWords.Queries.GetToday;
using Wordle.Application.DailyWords.Queries.Search;
using Wordle.Domain.DailyWords;

namespace Wordle.WebAPI.Controllers;

//[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class DailyWordsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DailyWordsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("today")]
    public async Task<ActionResult<GetDailyWordDto>> GetTodayWord()
    {
        var word = await _mediator.Send(new GetTodayWordQuery());

        if (word is null)
            return NotFound(new { error = "Bugüne ait kelime bulunamadı." });

        return Ok(word);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddDailyWordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Kelime başarıyla eklendi." });
    }   
   
    [HttpGet("past")]
    public async Task<ActionResult<DailyWordListResultDto>> GetPastWords([FromQuery] int page = 1, [FromQuery] int pageSize = 15)
    {
        var result = await _mediator.Send(new GetPastWordsQuery { Page = page, PageSize = pageSize });
        return Ok(result);
    }

    [HttpGet("planned")]
    public async Task<ActionResult<DailyWordListResultDto>> GetPlannedWords([FromQuery] int page = 1, [FromQuery] int pageSize = 15)
    {
        var result = await _mediator.Send(new GetPlannedWordsQuery { Page = page, PageSize = pageSize });
        return Ok(result);
    }

    [HttpDelete("{date}")]
    public async Task<IActionResult> Delete([FromRoute] string date)
    {
        if (!DateOnly.TryParseExact(date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
        {
            return BadRequest(new { error = "Tarih formatı geçersiz. Doğru format: dd.MM.yyyy" });
        }

        await _mediator.Send(new DeletePlannedWordCommand { Date = parsedDate });

        return Ok(new { message = "Kelime başarıyla silindi." });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateDailyWordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Kelime başarıyla güncellendi." });
    }

    
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string input)
    {
        var result = await _mediator.Send(new SearchDailyWordQuery(input));

        if (result == null)
        {
            if (DateOnly.TryParse(input, out _))
                return NotFound(new { error = "Seçtiğiniz tarihte kelime bulunmuyor." });

            return NotFound(new { error = "Bu kelime ile daha önce kayıt oluşturulmamış." });
        }

        return Ok(result);
    }

}
