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
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(
            CreateProjectRequest request)
        {
            var userId = GetCurrentUserId();

            var isMember = await _context.WorkspaceMembers
                .AnyAsync(member =>
                    member.WorkspaceId == request.WorkspaceId &&
                    member.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            var project = new Project
            {
                WorkspaceId = request.WorkspaceId,
                Name = request.Name,
                Description = request.Description
            };

            _context.Projects.Add(project);

            await _context.SaveChangesAsync();

            return Ok(project);
        }

        [HttpGet("workspace/{workspaceId}")]
        public async Task<IActionResult> GetProjects(Guid workspaceId)
        {
            var userId = GetCurrentUserId();

            var isMember = await _context.WorkspaceMembers
                .AnyAsync(member =>
                    member.WorkspaceId == workspaceId &&
                    member.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            var projects = await _context.Projects
                .Where(project => project.WorkspaceId == workspaceId)
                .Select(project => new
                {
                    project.Id,
                    project.Name,
                    project.Description,
                    project.CreatedAt
                })
                .ToListAsync();

            return Ok(projects);
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );
        }
    }
}