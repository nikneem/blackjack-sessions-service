using BlackJack.Sessions.Core.Abstractions.DataTransferObjects;
using BlackJack.Sessions.Core.Abstractions.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BlackJack.Sessions.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly IBlackJackSessionsService _service;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            
            if (Guid.TryParse(id, out Guid sessionId))
            {
                var sessionById = await _service.GetSessionByIdAsync(sessionId, ct);
                return Ok(sessionById);
            }

            var sessionByCode = await _service.GetSessionByCodeAsync(id, ct);
            return Ok(sessionByCode);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(SessionCreateDto dto, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var sessionDto = await _service.CreateSessionAsync(dto, ct);
            return Ok(sessionDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, SessionDetailsDto dto, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var sessionDto = await _service.UpdateSessionAsync(id, dto, ct);
            return Ok(sessionDto);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(Guid id, JsonPatchDocument<SessionDetailsDto> dto, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var sessionDto = await _service.PatchSessionAsync(id, dto, ct);
            return Ok(sessionDto);
        }


        public SessionsController(IBlackJackSessionsService service)
        {
            _service = service;
        }

    }
}
