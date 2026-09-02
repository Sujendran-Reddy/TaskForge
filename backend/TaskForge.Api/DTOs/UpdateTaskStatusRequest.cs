using TaskForge.Api.Models;

namespace TaskForge.Api.DTOs
{
    public class UpdateTaskStatusRequest
    {
        public TaskItemStatus Status { get; set; }
    }
}