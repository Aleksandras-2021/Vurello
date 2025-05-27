using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PSK.Server.Data.Entities;

namespace PSK.Server.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserTeamRole> UserTeamRoles { get; set; }
        public DbSet<JobHistory> JobHistories { get; set; }
        public DbSet<UserComment> Comments { get; set; }
        public DbSet<BoardColumn> BoardColumns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Team>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Team>()
                .Property(t => t.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Team>()
                .HasMany(t => t.Users)
                .WithMany(u => u.Teams);
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedTeams)
                .WithOne(t => t.Creator)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Team>()
                .Property(t => t.Version)
                .HasColumnName("xmin")
                .IsRowVersion();

            modelBuilder.Entity<Board>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Board>()
                .Property(b => b.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Board>()
                .HasOne(b => b.Team)
                .WithMany(t => t.Boards)
                .HasForeignKey(b => b.TeamId);


            modelBuilder.Entity<Board>()
                .Property(b => b.Version)
                .HasColumnName("xmin")
                .IsRowVersion();


            modelBuilder.Entity<Job>()
                .HasKey(j => j.Id);

            modelBuilder.Entity<Job>()
                .Property(j => j.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Job>()
                .HasOne(j => j.Board)
                .WithMany(b => b.Jobs)
                .HasForeignKey(t => t.BoardId);

            modelBuilder.Entity<Job>()
                .HasOne(j => j.AssignedMember)
                .WithMany(a => a.AssignedJobs)
                .HasForeignKey(t => t.AssignedMemberId);


            modelBuilder.Entity<JobHistory>()
                .HasKey(jh => jh.Id);

            modelBuilder.Entity<JobHistory>()
                .Property(jh => jh.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<JobHistory>()
                .HasOne(jh => jh.Job)
                .WithMany(j => j.JobHistories)
                .HasForeignKey(jh => jh.JobId);

            modelBuilder.Entity<JobHistory>()
                .HasOne(jh => jh.User)
                .WithMany()
                .HasForeignKey(jh => jh.UserId);

            modelBuilder.Entity<Job>()
                .Property(j => j.Version)
                .HasColumnName("xmin")
                .IsRowVersion();

            modelBuilder.Entity<Invitation>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Invitation>()
                .Property(i => i.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Recipient)
                .WithMany()
                .HasForeignKey(i => i.RecipientUserId);

            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Sender)
                .WithMany()
                .HasForeignKey(i => i.SenderUserId);

            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Team)
                .WithMany()
                .HasForeignKey(i => i.TeamId);


            modelBuilder.Entity<Label>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Label>()
                .Property(l => l.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Label>()
                .HasOne(i => i.Team)
                .WithMany(t => t.Labels)
                .HasForeignKey(i => i.TeamId);

            modelBuilder.Entity<Label>()
                .HasMany(j => j.Jobs)
                .WithMany(l => l.Labels);

            modelBuilder.Entity<Label>()
                .Property(l => l.Version)
                .HasColumnName("xmin")
                .IsRowVersion();


            modelBuilder.Entity<Role>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Role>()
                .Property(r => r.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Role>()
                .Property(r => r.Version)
                .HasColumnName("xmin")
                .IsRowVersion();

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles);

            modelBuilder.Entity<Role>()
                .HasOne(r => r.Team)
                .WithMany(t => t.Roles)
                .HasForeignKey(r => r.TeamId);

            modelBuilder.Entity<Permission>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Job", Description = "Allow creating, editing, deleting jobs" },
                new Permission { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Board", Description = "Allow creating, editing, deleting board" },
                new Permission { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Labels", Description = "Allow creating, editing, deleting labels" },
                new Permission { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Roles", Description = "Allow creating, editing, deleting roles and assign roles to users" },
                new Permission { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "TeamUsers", Description = "Allow inviting and removing users from team" },
                new Permission { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), Name = "Team", Description = "Allow to edit and delete team" }
            );


            modelBuilder.Entity<UserTeamRole>()
                .HasKey(utr => new { utr.UserId, utr.TeamId });

            modelBuilder.Entity<UserTeamRole>()
                .HasOne(utr => utr.User)
                .WithMany(u => u.UserTeamRoles)
                .HasForeignKey(utr => utr.UserId);

            modelBuilder.Entity<UserTeamRole>()
                .HasOne(utr => utr.Team)
                .WithMany(t => t.UserTeamRoles)
                .HasForeignKey(utr => utr.TeamId);

            modelBuilder.Entity<UserTeamRole>()
                .HasOne(utr => utr.Role)
                .WithMany(r => r.UserTeamRoles)
                .HasForeignKey(utr => utr.RoleId);


            modelBuilder.Entity<UserComment>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<UserComment>()
                .Property(r => r.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<UserComment>()
                .HasOne(r => r.Job)
                .WithMany(t => t.Comments)
                .HasForeignKey(r => r.JobId);

            modelBuilder.Entity<UserComment>()
                .HasOne(r => r.Creator)
                .WithMany(t => t.Comments)
                .HasForeignKey(r => r.CreatorId);

            modelBuilder.Entity<BoardColumn>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<BoardColumn>()
                .Property(r => r.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<BoardColumn>()
                .HasOne(r => r.Board)
                .WithMany(t => t.Columns)
                .HasForeignKey(r => r.BoardId);

            modelBuilder.Entity<BoardColumn>()
                .HasMany(r => r.Jobs)
                .WithOne(t => t.Column)
                .HasForeignKey(r => r.ColumnId);

            base.OnModelCreating(modelBuilder);
        }
    }
}