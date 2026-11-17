using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QSR.Application.Interfaces.Integrations;

namespace QSR.WebApi.Controllers;

[Authorize(Policy = "BranchPos")]
[ApiController]
[Route("api/integrations")]
public class IntegrationsController(IIntegrationResolver resolver) : ControllerBase
{
    [HttpGet("availability")]
    public IActionResult GetAvailability()
    {
        return Ok(resolver.GetAvailability().Select(item => new
        {
            Kind = item.Kind.ToString(),
            item.Available
        }));
    }
}
