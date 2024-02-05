using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedPoint.Application.Interfaces;
using MedPoint.Domain.Entities;

namespace MedPoint.Web.Controllers;

[Route("api/drug")]
[ApiController]
public class DrugController : ControllerBase
{
    private readonly IDrugService _drugService;

    public DrugController(IDrugService drugService)
    {
        _drugService = drugService;
    }

    /// <summary>
    /// Creates a new drug entry.
    /// </summary>
    /// <remarks>
    /// Only users with Admin or Contributor roles can create a new drug.
    /// </remarks>
    /// <param name="drug">The drug to create.</param>
    /// <returns>The created drug.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Contributor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateDrug([FromBody] Drug drug)
    {
        try
        {
            // Assuming userID extraction logic here
            var userId = new Guid(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var createdDrug = await _drugService.CreateDrugAsync(drug, userId);
            return CreatedAtAction(nameof(GetDrugById), new { id = createdDrug.Id }, createdDrug);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing drug entry.
    /// </summary>
    /// <param name="id">The ID of the drug to update.</param>
    /// <param name="drug">The updated drug information.</param>
    /// <returns>A status indicating the update was successful.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Contributor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateDrug(Guid id, [FromBody] Drug drug)
    {
        try
        {
            var userId = new Guid(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            await _drugService.UpdateDrugAsync(drug, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a drug entry.
    /// </summary>
    /// <param name="id">The ID of the drug to delete.</param>
    /// <returns>A status indicating the deletion was successful.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Contributor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteDrug(Guid id)
    {
        try
        {
            var userId = new Guid(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            await _drugService.DeleteDrugAsync(id, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a drug by its ID.
    /// </summary>
    /// <param name="id">The ID of the drug to retrieve.</param>
    /// <returns>The requested drug.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDrugById(Guid id)
    {
        var drug = await _drugService.GetDrugByIdAsync(id);
        if (drug == null)
        {
            return NotFound();
        }
        return Ok(drug);
    }

    /// <summary>
    /// Lists drugs with optional search term and pagination.
    /// </summary>
    /// <param name="searchTerm">The search term to filter drugs.</param>
    /// <param name="pageNumber">The page number to retrieve, starting at 1.</param>
    /// <param name="pageSize">The number of drugs per page. Defaults to 10.</param>
    /// <returns>A list of drugs and the total count for pagination.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListDrugs([FromQuery] string searchTerm = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchResults = await _drugService.SearchDrugsAsync(searchTerm);
            return Ok(searchResults);
        }
        else
        {
            var drugs = await _drugService.ListDrugsAsync(pageSize, (pageNumber - 1) * pageSize);
            return Ok(drugs);
        }
    }
}
