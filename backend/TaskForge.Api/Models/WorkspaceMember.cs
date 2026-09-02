namespace TaskForge.Api.Models
{
    public class WorkspaceMember
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid WorkspaceId { get; set; }

        public Workspace Workspace { get; set; } = null!;

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public WorkspaceRole Role { get; set; } = WorkspaceRole.Member;
    }
}