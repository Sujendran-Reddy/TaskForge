using TaskForge.Api.Models;

namespace TaskForge.Api.DTOs
{
    public class AddWorkspaceMemberRequest
    {
        public string Email { get; set; } = string.Empty;

        public WorkspaceRole Role { get; set; } = WorkspaceRole.Member;
    }
}