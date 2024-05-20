using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace VolunteerHub.DataModels.Models;

public partial class VolunteerHubContext : IdentityDbContext<User, IdentityRole, string>
{
    public VolunteerHubContext()
    {
    }

    public VolunteerHubContext(DbContextOptions<VolunteerHubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectStat> ProjectStats { get; set; }

    public virtual DbSet<ProjectTask> ProjectTasks { get; set; }

    public virtual DbSet<UserOrganization> UserOrganizations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserTask> UserTasks { get; set; }
    public virtual DbSet<UserStat> UserStats { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-EDQ5MCB;Initial Catalog=VolunteerHub;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("announcement_id_primary");

            entity.ToTable("Announcement");

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Project).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("announcement_projectid_foreign");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_id_primary");

            entity.ToTable("Notification");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Subject).HasMaxLength(255);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("organization_id_primary");

            entity.ToTable("Organization");

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.Adress).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Contact).HasMaxLength(255).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Code).HasColumnName("Code");
            entity.HasOne(o => o.User).WithMany(u => u.Organizations).HasForeignKey(o => o.OwnerId);
        });

        
            modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("project_id_primary");
            entity.ToTable("Project");
            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.ProjectName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EndDate)
                .HasColumnType("date")
                .HasColumnName("endDate");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.HasOne(u => u.Owner).WithMany(p => p.Projects).HasForeignKey(u => u.OwnerId);

            entity
            .HasOne(o => o.Organization)
            .WithMany(p => p.Projects).
            HasForeignKey(p => p.OrganizationId);
            

            
        });

        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserTask_id_primary");
            entity.ToTable("UserTask");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(e => e.Task).WithMany(p => p.UserTasks)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_UserTask_Task");

            entity.HasOne(e => e.User).WithMany(p => p.UserTasks)
               .HasForeignKey(d => d.UserId)
               .HasConstraintName("FK_UserTask_User");
        });

        modelBuilder.Entity<UserOrganization>(entity => {
            entity.HasKey(e => e.Id).HasName("UserOrganization_id_primary");
            entity.ToTable("UserOrganization");
            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(e => e.Organization).WithMany(p => p.UserOrganizations)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK_UserOrganization_organization");

            entity.HasOne(e => e.User).WithMany(p => p.UserOrganizations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserOrganization_User");
        });

        modelBuilder.Entity<ProjectStat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("projectstats_id_primary");

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectStats)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("projectstats_projectid_foreign");
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("projecttask_id_primary");
            entity.ToTable("ProjectTask");
            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.Action).HasMaxLength(255);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.MeasureUnit).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("projecttask_projectid_foreign");
            entity.Property(e => e.Progress).HasPrecision(18, 2);
        });

        modelBuilder.Entity<User>(entity =>
        { 
            entity.HasOne(u => u.Project).WithMany(p => p.Users).HasForeignKey(u => u.ProjectId).OnDelete(DeleteBehavior.ClientSetNull);
        });
        

        modelBuilder.Entity<UserStat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userstats_id_primary");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        AddTimestamps();
        return base.SaveChangesAsync();
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity.GetType().GetProperty("CreatedAt") != null && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow; // current datetime

            if (entity.State == EntityState.Added)
            {
                entity.Entity.GetType().GetProperty("CreatedAt").SetValue(entity.Entity, now);
            }    
                entity.Entity.GetType().GetProperty("UpdatedAt").SetValue(entity.Entity, now);
            
        }
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


}
