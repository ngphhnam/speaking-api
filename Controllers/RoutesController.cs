using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllRoutes([FromServices] EndpointDataSource endpointDataSource)
    {
        var routes = endpointDataSource.Endpoints
            .OfType<RouteEndpoint>()
            .Select(e => new
            {
                Path = e.RoutePattern.RawText ?? string.Join("/", e.RoutePattern.PathSegments.Select(s => s.ToString())),
                Method = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods.FirstOrDefault() ?? "GET",
                DisplayName = e.DisplayName
            })
            .OrderBy(e => e.Path)
            .ToList();

        return Ok(new
        {
            total = routes.Count,
            baseUrl = $"{Request.Scheme}://{Request.Host}",
            routes = routes
        });
    }
}

