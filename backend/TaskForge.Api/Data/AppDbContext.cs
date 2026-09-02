using Microsoft.EntityFrameworkCore;
using TaskForge.Api.Models;

namespace TaskForge.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Workspace> Workspaces { get; set; }

        public DbSet<WorkspaceMember> WorkspaceMembers { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();

            modelBuilder.Entity<WorkspaceMember>()
                .HasIndex(member => new
                {
                    member.WorkspaceId,
                    member.UserId
                })
                .IsUnique();

            modelBuilder.Entity<Workspace>()
                .HasOne(workspace => workspace.Owner)
                .WithMany()
                .HasForeignKey(workspace => workspace.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(task => task.Creator)
                .WithMany()
                .HasForeignKey(task => task.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(task => task.Assignee)
                .WithMany()
                .HasForeignKey(task => task.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}