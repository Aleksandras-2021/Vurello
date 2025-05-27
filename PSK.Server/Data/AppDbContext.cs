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
        public DbSet<JobHistory> JobHistories { get; set; }

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
                .HasMany(t => t.Users)
                .WithMany(u => u.Teams);

            modelBuilder.Entity<Board>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Board>()
                .Property(b => b.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Board>()
                .HasOne(b => b.Team)
                .WithMany(t => t.Boards)
                .HasForeignKey(b => b.TeamId);

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

            modelBuilder.Entity<Team>()
                .Property(t => t.Version)
                .HasColumnName("xmin")
                .IsRowVersion();

            modelBuilder.Entity<Board>()
                .Property(b => b.Version)
                .HasColumnName("xmin")
                .IsRowVersion();

            modelBuilder.Entity<Job>()
                .Property(j => j.Version)
                .HasColumnName("xmin")
                .IsRowVersion();

            modelBuilder.Entity<Label>()
                .Property(l => l.Version)
                .HasColumnName("xmin")
                .IsRowVersion();

            base.OnModelCreating(modelBuilder);
        }
    }
}