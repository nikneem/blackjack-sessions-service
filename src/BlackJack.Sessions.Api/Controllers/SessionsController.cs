using BlackJack.Sessions.Core.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlackJack.Sessions.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly IBlackJackSessionsService _service;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id, CancellationToken ct = default)
        {
            if (Guid.TryParse(id, out Guid sessionId))
            {
                var sessionById = await _service.GetSessionByIdAsync(sessionId, ct);
                return Ok(sessionById);
            }

            var sessionByCode = await _service.GetSessionByCodeAsync(id, ct);
            return Ok(sessionByCode);
        }

        public SessionsController(IBlackJackSessionsService service)
        {
            _service = service;
        }

    }
}
