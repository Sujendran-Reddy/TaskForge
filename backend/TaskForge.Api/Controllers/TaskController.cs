using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskForge.Api.Data;
using TaskForge.Api.DTOs;
using TaskForge.Api.Models;

namespace TaskForge.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(
            CreateTaskRequest request)
        {
            var userId = GetCurrentUserId();

            var project = await _context.Projects
                .FirstOrDefaultAsync(project =>
                    project.Id == request.ProjectId);

            if (project == null)
            {
                return NotFound(new
                {
                    message = "Project not found."
                });
            }

            var isMember = await _context.WorkspaceMembers
                .AnyAsync(member =>
                    member.WorkspaceId == project.WorkspaceId &&
                    member.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            if (request.AssigneeId.HasValue)
            {
                var assigneeIsMember = await _context.WorkspaceMembers
                    .AnyAsync(member =>
                        member.WorkspaceId == project.WorkspaceId &&
                        member.UserId == request.AssigneeId.Value);

                if (!assigneeIsMember)
                {
                    return BadRequest(new
                    {
                        message =
                            "Assignee must be a member of the workspace."
                    });
                }
            }

            var task = new TaskItem
            {
                ProjectId = request.ProjectId,
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                DueDate = request.DueDate,
                AssigneeId = request.AssigneeId,
                CreatorId = userId
            };

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                task.Id,
                task.Title,
                task.Description,
                task.Priority,
                task.Status,
                task.DueDate,
                task.AssigneeId,
                task.CreatorId,
                task.CreatedAt
            });
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasks(Guid projectId)
        {
            var userId = GetCurrentUserId();

            var project = await _context.Projects
                .FirstOrDefaultAsync(project =>
                    project.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            var isMember = await _context.WorkspaceMembers
                .AnyAsync(member =>
                    member.WorkspaceId == project.WorkspaceId &&
                    member.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            var tasks = await _context.Tasks
                .Where(task => task.ProjectId == projectId)
                .Select(task => new
                {
                    task.Id,
                    task.Title,
                    task.Description,
                    task.Priority,
                    task.Status,
                    task.DueDate,
                    task.AssigneeId,
                    AssigneeName = task.Assignee != null
                        ? task.Assignee.Name
                        : null,
                    task.CreatorId,
                    task.CreatedAt,
                    task.UpdatedAt
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpPatch("{taskId}/status")]
        public async Task<IActionResult> UpdateStatus(
            Guid taskId,
            UpdateTaskStatusRequest request)
        {
            var userId = GetCurrentUserId();

            var task = await _context.Tasks
                .Include(task => task.Project)
                .FirstOrDefaultAsync(task => task.Id == taskId);

            if (task == null)
            {
                return NotFound();
            }

            var isMember = await _context.WorkspaceMembers
                .AnyAsync(member =>
                    member.WorkspaceId == task.Project.WorkspaceId &&
                    member.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            task.Status = request.Status;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                task.Id,
                task.Title,
                task.Status,
                task.UpdatedAt
            });
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );
        }
    }
}