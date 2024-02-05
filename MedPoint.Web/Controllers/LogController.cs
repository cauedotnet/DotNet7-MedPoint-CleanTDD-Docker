using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedPoint.Application.Interfaces;

namespace MedPoint.Web.Controllers;

[Route("api/logs")]
[ApiController]
public class LogController : ControllerBase
{
    private readonly ILogService _logService;

    public LogController(ILogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    /// Lists the latest logs with pagination.
    /// </summary>
    /// <remarks>
    /// This endpoint is accessible only to users with the Admin role.
    /// </remarks>
    /// <param name="pageNumber">The page number to retrieve, starting at 1.</param>
    /// <param name="pageSize">The number of logs per page. Defaults to 10.</param>
    /// <returns>A list of logs and the total count for pagination.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListLatestLogs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var logs = await _logService.ListLatestLogsAsync(pageSize, (pageNumber - 1) * pageSize);

        return Ok(logs);
    }
}