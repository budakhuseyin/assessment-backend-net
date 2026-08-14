using ContactService.Application.DTOs;
using ContactService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactService.API.Controllers;

/// <summary>
/// Bir kişiye ait iletişim bilgilerinin (telefon, email, konum) yönetimi için HTTP endpoint'leri.
/// Ortak davranışlar BaseApiController'dan miras alınır.
/// </summary>
[Route("api/person/{personId:guid}/contact")]
public class ContactInfoController : BaseApiController
{
    private readonly IContactInfoService _contactInfoService;

    public ContactInfoController(IContactInfoService contactInfoService)
    {
        _contactInfoService = contactInfoService;
    }

    /// <summary>
    /// Belirli bir kişiye yeni iletişim bilgisi ekler.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Add(Guid personId, [FromBody] CreateContactInfoRequest request)
    {
        try
        {
            var result = await _contactInfoService.AddContactInfoAsync(personId, request);
            return Created(nameof(Add), new { personId }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFoundResult(ex.Message);
        }
    }

    /// <summary>
    /// Belirli bir iletişim bilgisini siler.
    /// </summary>
    [HttpDelete("{contactInfoId:guid}")]
    public async Task<IActionResult> Delete(Guid personId, Guid contactInfoId)
    {
        var result = await _contactInfoService.DeleteContactInfoAsync(contactInfoId);
        if (!result) return NotFoundResult();
        return NoContent();
    }
}
