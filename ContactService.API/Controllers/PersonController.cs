using ContactService.Application.DTOs;
using ContactService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactService.API.Controllers;

/// <summary>
/// Kişi (rehber kaydı) yönetimi için HTTP endpoint'leri sağlayan controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly IPersonService _personService;

    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>
    /// Tüm kişileri listeler.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var persons = await _personService.GetAllAsync();
        return Ok(persons);
    }

    /// <summary>
    /// UUID ile tek bir kişinin tüm detaylarını (iletişim bilgileri dahil) getirir.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var person = await _personService.GetByIdAsync(id);
        if (person == null) return NotFound();
        return Ok(person);
    }

    /// <summary>
    /// Yeni bir kişi oluşturur.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonRequest request)
    {
        var created = await _personService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.UUID }, created);
    }

    /// <summary>
    /// UUID ile bir kişiyi (ve tüm iletişim bilgilerini) siler.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _personService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
