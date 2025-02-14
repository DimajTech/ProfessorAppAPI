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

    public virtual DbSet<User> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advisement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Adviseme__3214EC07C37FE190");

            entity.ToTable("Advisement");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.CourseId).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.StudentId).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.Advisements)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("fk_Advisement_Course");

            entity.HasOne(d => d.User).WithMany(p => p.Advisements)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("fk_Advisement_User");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC07F79E016E");

            entity.ToTable("Appointment");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.CourseId).HasMaxLength(50);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Mode).HasMaxLength(20);
            entity.Property(e => e.ProfessorComment).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.StudentId).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("fk_Appointment_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("fk_Appointment_User");
        });

        modelBuilder.Entity<CommentNews>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CommentN__3214EC07B3496E6B");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.AuthorId).HasMaxLength(50);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.PieceOfNewsId).HasMaxLength(50);

            entity.HasOne(d => d.Author).WithMany(p => p.CommentNews)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("fk_CommentNews_User");

            entity.HasOne(d => d.PieceOfNews).WithMany(p => p.CommentNews)
                .HasForeignKey(d => d.PieceOfNewsId)
                .HasConstraintName("fk_CommentNews_PieceOfNews");
        });

        modelBuilder.Entity<CommentNewsResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CommentN__3214EC0728436ED6");

            entity.ToTable("CommentNewsResponse");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.AuthorId).HasMaxLength(50);
            entity.Property(e => e.CommentNewsId).HasMaxLength(50);
            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.Author).WithMany(p => p.CommentNewsResponses)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("fk_CommentNewsResponses_User");

            entity.HasOne(d => d.CommentNews).WithMany(p => p.CommentNewsResponses)
                .HasForeignKey(d => d.CommentNewsId)
                .HasConstraintName("fk_CommentNewsResponses_CommentNews");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course__3214EC0739B95343");

            entity.ToTable("Course");

            entity.HasIndex(e => e.Code, "UQ__Course__A25C5AA75E7DB04D").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ProfessorId).HasMaxLength(50);
            entity.Property(e => e.Semester).HasMaxLength(20);

            entity.HasOne(d => d.Professor).WithMany(p => p.Courses)
                .HasForeignKey(d => d.ProfessorId)
                .HasConstraintName("fk_Course_User");
        });

        modelBuilder.Entity<PieceOfNews>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PieceOfN__3214EC0727785666");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.AuthorId).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Author).WithMany(p => p.PieceOfNews)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("fk_PieceOfNews_User");
        });

        modelBuilder.Entity<ResponseAdvisement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Response__3214EC07CE1333BE");

            entity.ToTable("ResponseAdvisement");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.AdvisementId).HasMaxLength(50);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(50);

            entity.HasOne(d => d.Advisement).WithMany(p => p.ResponseAdvisements)
                .HasForeignKey(d => d.AdvisementId)
                .HasConstraintName("fk_ResponseAdvisement_Advisement");

            entity.HasOne(d => d.User).WithMany(p => p.ResponseAdvisements)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_ResponseAdvisement_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0739E882C8");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasMaxLength(50);
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
