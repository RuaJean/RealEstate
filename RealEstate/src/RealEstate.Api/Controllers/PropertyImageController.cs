using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Services;
using RealEstate.Application.DTOs.PropertyImage;

namespace RealEstate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyImageController : ControllerBase
    {
        private readonly IPropertyImageService _service;

        public PropertyImageController(IPropertyImageService service)
        {
            _service = service;
        }

        [HttpGet("by-property/{propertyId:guid}")]
        public async Task<ActionResult<IEnumerable<PropertyImageResponseDto>>> GetByProperty(Guid propertyId, CancellationToken ct)
        {
            var list = await _service.GetByPropertyIdAsync(propertyId, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<PropertyImageResponseDto>> Create([FromBody] PropertyImageCreateDto req, CancellationToken ct)
        {
            var created = await _service.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetByProperty), new { propertyId = created.PropertyId }, created);
        }

        [HttpPatch("{id:guid}/enabled")]
        public async Task<IActionResult> SetEnabled(Guid id, [FromQuery] bool enabled, CancellationToken ct)
        {
            var ok = await _service.SetEnabledAsync(id, enabled, ct);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var ok = await _service.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}


