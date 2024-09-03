using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DataContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Document> Documents { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Workspace)
                .WithOne(w => w.User)
                .HasForeignKey<Workspace>(w => w.UserId);

            modelBuilder.Entity<User>()
           .HasIndex(u => u.Email)
           .IsUnique();

            modelBuilder.Entity<User>()
           .HasIndex(u => u.UserName)
           .IsUnique();

            // Configure Workspace
            modelBuilder.Entity<Workspace>()
                .HasMany(w => w.Folders)
                .WithOne(f => f.Workspace)
                .HasForeignKey(f => f.WorkspaceId);

            modelBuilder.Entity<Workspace>()
           .Property(w => w.Name)
           .IsRequired()
           .HasMaxLength(100);

            modelBuilder.Entity<Workspace>()
           .HasIndex(w => w.Name )
           .IsUnique();

            //Configure Folder
            modelBuilder.Entity<Folder>()
            .HasMany(f => f.Documents)
            .WithOne(d => d.Folder)
            .HasForeignKey(d => d.FolderId);

            modelBuilder.Entity<Folder>()
            .Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

            //Configure Document
            modelBuilder.Entity<Document>()
            .Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(255);

            modelBuilder.Entity<Document>()
                .Property(d => d.Type)
                .HasMaxLength(50);



        }
    }
}
