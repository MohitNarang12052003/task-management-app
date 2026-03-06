using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs;
using TaskManagement.Api.Services;

using Microsoft.AspNetCore.RateLimiting;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
[EnableRateLimiting("fixed")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? User.FindFirstValue("sub")
                          ?? throw new UnauthorizedAccessException("User ID not found in token.");

    /// <summary>Retrieves all tasks for the authenticated user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllTasksAsync(UserId));

    /// <summary>Retrieves a task by ID (must belong to the authenticated user).</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var task = await _service.GetTaskByIdAsync(id, UserId);
        if (task is null)
            return NotFound(new { message = $"Task with id '{id}' was not found." });
        return Ok(task);
    }

    /// <summary>Creates a new task for the authenticated user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreateTaskAsync(dto, UserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Updates an existing task (must belong to the authenticated user).</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTaskDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await _service.UpdateTaskAsync(id, dto, UserId);
        if (!updated)
            return NotFound(new { message = $"Task with id '{id}' was not found." });
        return NoContent();
    }

    /// <summary>Deletes a task (must belong to the authenticated user).</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _service.DeleteTaskAsync(id, UserId);
        if (!deleted)
            return NotFound(new { message = $"Task with id '{id}' was not found." });
        return NoContent();
    }
}
