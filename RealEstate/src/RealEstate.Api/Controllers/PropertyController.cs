using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs.Property;
using RealEstate.Application.Services;

namespace RealEstate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _service;

        public PropertyController(IPropertyService service)
        {
            _service = service;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PropertyResponseDto>> GetById(Guid id, CancellationToken ct)
        {
            var entity = await _service.GetByIdAsync(id, ct);
            if (entity is null) return NotFound();
            return Ok(entity);
        }

        [HttpGet]
        public async Task<ActionResult<object>> Search([FromQuery] Guid? ownerId, [FromQuery] string? text, [FromQuery] decimal? priceMin, [FromQuery] decimal? priceMax, [FromQuery] int? year, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var filter = new PropertyFilterDto { OwnerId = ownerId, Text = text, PriceMin = priceMin, PriceMax = priceMax, Year = year, Page = page, PageSize = pageSize };
            var result = await _service.SearchPagedAsync(filter, ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PropertyResponseDto>> Create([FromBody] PropertyCreateDto req, CancellationToken ct)
        {
            var created = await _service.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PropertyUpdateDto req, CancellationToken ct)
        {
            var ok = await _service.UpdateAsync(id, req, ct);
            return ok ? NoContent() : Problem("No se pudo actualizar");
        }

        [HttpPatch("{id:guid}/price")]
        public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] PropertyPriceUpdateDto req, CancellationToken ct)
        {
            var ok = await _service.UpdatePriceAsync(id, req, ct);
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


