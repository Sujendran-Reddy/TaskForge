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
    public class WorkspacesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkspacesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkspace(
            CreateWorkspaceRequest request)
        {
            var userId = GetCurrentUserId();

            var workspace = new Workspace
            {
                Name = request.Name,
                OwnerId = userId
            };

            _context.Workspaces.Add(workspace);

            _context.WorkspaceMembers.Add(new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = userId,
                Role = WorkspaceRole.Owner
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                workspace.Id,
                workspace.Name,
                workspace.OwnerId,
                workspace.CreatedAt
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWorkspaces()
        {
            var userId = GetCurrentUserId();

            var workspaces = await _context.WorkspaceMembers
                .Where(member => member.UserId == userId)
                .Select(member => new
                {
                    member.Workspace.Id,
                    member.Workspace.Name,
                    member.Role,
                    member.Workspace.CreatedAt
                })
                .ToListAsync();

            return Ok(workspaces);
        }

        [HttpPost("{workspaceId}/members")]
        public async Task<IActionResult> AddMember(
            Guid workspaceId,
            AddWorkspaceMemberRequest request)
        {
            var currentUserId = GetCurrentUserId();

            var currentMember = await _context.WorkspaceMembers
                .FirstOrDefaultAsync(member =>
                    member.WorkspaceId == workspaceId &&
                    member.UserId == currentUserId);

            if (currentMember == null ||
                (currentMember.Role != WorkspaceRole.Owner &&
                 currentMember.Role != WorkspaceRole.Admin))
            {
                return Forbid();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Email == request.Email);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            var alreadyMember = await _context.WorkspaceMembers
                .AnyAsync(member =>
                    member.WorkspaceId == workspaceId &&
                    member.UserId == user.Id);

            if (alreadyMember)
            {
                return BadRequest(new
                {
                    message = "User is already a workspace member."
                });
            }

            var member = new WorkspaceMember
            {
                WorkspaceId = workspaceId,
                UserId = user.Id,
                Role = request.Role
            };

            _context.WorkspaceMembers.Add(member);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Member added successfully.",
                user.Id,
                user.Name,
                user.Email,
                member.Role
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