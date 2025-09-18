using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Services;
using RealEstate.Application.DTOs.PropertyTrace;

namespace RealEstate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyTraceController : ControllerBase
    {
        private readonly IPropertyTraceService _service;

        public PropertyTraceController(IPropertyTraceService service)
        {
            _service = service;
        }

        [HttpGet("by-property/{propertyId:guid}")]
        public async Task<ActionResult<IEnumerable<PropertyTraceResponseDto>>> GetByProperty(Guid propertyId, CancellationToken ct)
        {
            var list = await _service.GetByPropertyIdAsync(propertyId, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<PropertyTraceResponseDto>> Create([FromBody] PropertyTraceCreateDto req, CancellationToken ct)
        {
            var created = await _service.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetByProperty), new { propertyId = created.PropertyId }, created);
        }
    }
}


