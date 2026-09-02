namespace TaskForge.Api.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

        public DateTime? DueDate { get; set; }

        public Guid ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public Guid CreatorId { get; set; }

        public User Creator { get; set; } = null!;

        public Guid? AssigneeId { get; set; }

        public User? Assignee { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}