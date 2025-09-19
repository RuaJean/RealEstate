using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RealEstate.Application.DTOs.Property;
using RealEstate.Application.DTOs.PropertyImage;
using RealEstate.Application.Services;
using RealEstate.Application.Interfaces;
using RealEstate.Api.Models;

namespace RealEstate.Api.Controllers
{
    [ApiController]
    [Route("api/properties")]
    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _service;
        private readonly IPropertyImageService _imageService;
        private readonly IFileStorageService _storage;

        public PropertyController(IPropertyService service, IPropertyImageService imageService, IFileStorageService storage)
        {
            _service = service;
            _imageService = imageService;
            _storage = storage;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PropertyResponseDto>> GetById(Guid id, CancellationToken ct)
        {
            var entity = await _service.GetByIdAsync(id, ct);
            if (entity is null) return NotFound();
            return Ok(entity);
        }

        [HttpGet]
        [AllowAnonymous]
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
            // Si no se pudo actualizar, lo más probable es que no exista el recurso
            return ok ? NoContent() : NotFound();
        }

        [HttpPatch("{id:guid}/price")]
        public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] PropertyPriceUpdateDto req, CancellationToken ct)
        {
            var ok = await _service.UpdatePriceAsync(id, req, ct);
            return ok ? NoContent() : NotFound();
        }

        [HttpPut("{id:guid}/price")]
        public async Task<IActionResult> UpdatePricePut(Guid id, [FromBody] PropertyPriceUpdateDto req, CancellationToken ct)
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

        [HttpPost("{id:guid}/images")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(104857600)] // 100MB
        public async Task<ActionResult<PropertyImageResponseDto>> AddImage(Guid id, [FromForm] PropertyImageUploadRequest form, CancellationToken ct = default)
        {
            try
            {
                var file = form.File;
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Archivo vacío");
                }

                await using var stream = file.OpenReadStream();
                var relative = await _storage.SaveFileAsync(stream, file.FileName, file.ContentType, ct);
                var publicPath = _storage.GetPublicUrl(relative);
                var url = $"{Request.Scheme}://{Request.Host}{publicPath}";

                var created = await _imageService.CreateAsync(new PropertyImageCreateDto
                {
                    PropertyId = id,
                    Url = url,
                    Description = form.Description ?? string.Empty,
                    Enabled = form.Enabled
                }, ct);

                return Created(url, created);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}


