using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all tasks.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _service.GetAllTasksAsync();
        return Ok(tasks);
    }

    /// <summary>
    /// Retrieves a task by its ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var task = await _service.GetTaskByIdAsync(id);
        if (task is null)
            return NotFound(new { message = $"Task with id '{id}' was not found." });

        return Ok(task);
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateTaskAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _service.UpdateTaskAsync(id, dto);
        if (!updated)
            return NotFound(new { message = $"Task with id '{id}' was not found." });

        return NoContent();
    }

    /// <summary>
    /// Deletes a task by its ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _service.DeleteTaskAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Task with id '{id}' was not found." });

        return NoContent();
    }
}
