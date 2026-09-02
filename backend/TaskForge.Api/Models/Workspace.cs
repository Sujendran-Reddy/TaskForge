namespace TaskForge.Api.Models
{
    public class Workspace
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }

        public User Owner { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<WorkspaceMember> Members { get; set; } = new();

        public List<Project> Projects { get; set; } = new();
    }
}