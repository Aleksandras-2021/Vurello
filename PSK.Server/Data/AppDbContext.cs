using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PSK.Server.Data.Entities;

namespace PSK.Server.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Board> Boards { get; set; }
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


            modelBuilder.Entity<Board>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Board>()
                .Property(b => b.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Board>()
                .HasOne(b => b.Team)
                .WithMany(t => t.Boards)
                .HasForeignKey(b => b.TeamId);



            base.OnModelCreating(modelBuilder);
        }

    }
}
