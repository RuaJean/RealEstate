using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs.Owner;
using RealEstate.Application.Services;

namespace RealEstate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _service;

        public OwnerController(IOwnerService service)
        {
            _service = service;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OwnerResponseDto>> GetById(Guid id, CancellationToken ct)
        {
            var owner = await _service.GetByIdAsync(id, ct);
            if (owner is null) return NotFound();
            return Ok(owner);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OwnerResponseDto>>> Search([FromQuery] string? name, [FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var list = await _service.SearchAsync(name, skip, take, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<OwnerResponseDto>> Create([FromBody] OwnerCreateDto req, CancellationToken ct)
        {
            var created = await _service.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OwnerUpdateDto req, CancellationToken ct)
        {
            var ok = await _service.UpdateAsync(id, req, ct);
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
