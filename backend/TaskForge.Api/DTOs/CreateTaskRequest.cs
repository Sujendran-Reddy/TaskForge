using TaskForge.Api.Models;

namespace TaskForge.Api.DTOs
{
    public class CreateTaskRequest
    {
        public Guid ProjectId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime? DueDate { get; set; }

        public Guid? AssigneeId { get; set; }
    }
}