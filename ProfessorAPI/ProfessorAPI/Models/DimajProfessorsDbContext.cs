using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProfessorAPI.Models;

public partial class DimajProfessorsDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DimajProfessorsDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Advisement> Advisements { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<CommentNews> CommentNews { get; set; }

    public virtual DbSet<CommentNewsResponse> CommentNewsResponses { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<PieceOfNews> PieceOfNews { get; set; }

    public virtual DbSet<ResponseAdvisement> ResponseAdvisements { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advisement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Adviseme__3214EC07E1CC50A4");

            entity.ToTable("Advisement");

            entity.Property(e => e.Id).HasMaxLength(30);
            entity.Property(e => e.CourseId).HasMaxLength(30);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.StudentId).HasMaxLength(30);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC07A865C540");

            entity.ToTable("Appointment");

            entity.Property(e => e.Id).HasMaxLength(30);
            entity.Property(e => e.CourseId).HasMaxLength(30);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Mode).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.StudentId).HasMaxLength(30);
        });

        modelBuilder.Entity<CommentNews>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CommentN__3214EC07C120D468");

            entity.Property(e => e.Id).HasMaxLength(30);
            entity.Property(e => e.AuthorId).HasMaxLength(30);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.PieceOfNewsId).HasMaxLength(30);
        });

        modelBuilder.Entity<CommentNewsResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CommentN__3214EC07216B135E");

            entity.ToTable("CommentNewsResponse");

            entity.Property(e => e.Id).HasMaxLength(30);
            entity.Property(e => e.AuthorId).HasMaxLength(30);
            entity.Property(e => e.CommentNewsId).HasMaxLength(30);
            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.Author).WithMany(p => p.CommentNewsResponses)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_CommentNewsResponses_User");

            entity.HasOne(d => d.CommentNews).WithMany(p => p.CommentNewsResponses)
                .HasForeignKey(d => d.CommentNewsId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_CommentNewsResponses_CommentNews");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC070E1F8E0B");

            entity.ToTable("Course");

            entity.HasIndex(e => e.Code, "UQ__Course__A25C5AA75C4F1269").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(30);
            entity.Property(e => e.Code).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.ProfessorId).HasMaxLength(30);
            entity.Property(e => e.Semester).HasMaxLength(20);
        });

        modelBuilder.Entity<PieceOfNews>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PieceOfN__3214EC0776537754");

            entity.Property(e => e.Id).HasMaxLength(30);
            entity.Property(e => e.AuthorId).HasMaxLength(30);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Author).WithMany(p => p.PieceOfNews)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_PieceOfNews_User");
        });

        modelBuilder.Entity<ResponseAdvisement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Response__3214EC07542CF84C");

            entity.ToTable("ResponseAdvisement");

            entity.Property(e => e.Id).HasMaxLength(30);
            entity.Property(e => e.AdvisementId).HasMaxLength(30);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(30);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0765FC9758");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasMaxLength(30);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RegistrationStatus)
                .HasMaxLength(50)
                .HasColumnName("Registration_Status");
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
