using Microsoft.EntityFrameworkCore;
using Service_Managment.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Service_Managment.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Worker> Workers => Set<Worker>();
        public DbSet<Admin> Admins => Set<Admin>();
        //public DbSet<WorkerSchedule> WorkerSchedules => Set<WorkerSchedule>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();
        public DbSet<Complaint> Complaints => Set<Complaint>();
        public DbSet<Rating> Ratings => Set<Rating>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- Users ----
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserEmail).IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.MobileNo).IsUnique();

            // ---- Worker (1-1 with User) ----
            modelBuilder.Entity<Worker>()
                .HasOne(w => w.User)
                .WithOne(u => u.WorkerProfile)
                .HasForeignKey<Worker>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- Admin (1-1 with User) ----
            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.AdminProfile)
                .HasForeignKey<Admin>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- WorkerSchedule (many-1 with Worker) ----
           

            // WorkerSchedule <-> Appointment (1-1, once booked)
            

            // ---- Appointment ----
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Worker)
                .WithMany()
                .HasForeignKey(a => a.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            // speeds up the overlap/double-booking check query
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.WorkerId, a.ScheduledStart, a.ScheduledEnd });

            // ---- Payment (many-1 with Appointment) ----
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Appointment)
                .WithMany(a => a.Payments)
                .HasForeignKey(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            
                // ----paymet (many-1 with Appointment) ----

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            //----OtpVerification(many - 1 with Appointment)----
            modelBuilder.Entity<OtpVerification>()
                .HasOne(o => o.Appointment)
                .WithMany(a => a.OtpVerifications)
                .HasForeignKey(o => o.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- Complaint (1-1 with Appointment) ----
            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Appointment)
                .WithOne(a => a.Complaint)
                .HasForeignKey<Complaint>(c => c.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- Rating (1-1 with Appointment, many-1 with User/Worker) ----
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Appointment)
                .WithOne(a => a.RatingDetails)
                .HasForeignKey<Rating>(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Worker)
                .WithMany()
                .HasForeignKey(r => r.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- Notification (many-1 with User) ----
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- Store enums as strings (readable in SQL Server instead of raw ints) ----
            modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();
            modelBuilder.Entity<Worker>().Property(w => w.Category).HasConversion<string>();
            modelBuilder.Entity<Worker>().Property(w => w.PricingModel).HasConversion<string>();
            modelBuilder.Entity<Worker>().Property(w => w.VerificationStatus).HasConversion<string>();
            modelBuilder.Entity<Appointment>().Property(a => a.Status).HasConversion<string>();
            modelBuilder.Entity<Payment>().Property(p => p.Method).HasConversion<string>();
            modelBuilder.Entity<Payment>().Property(p => p.Status).HasConversion<string>();
            //modelBuilder.Entity<OtpVerification>().Property(o => o.Type).HasConversion<string>();
            modelBuilder.Entity<Complaint>().Property(c => c.Status).HasConversion<string>();
            modelBuilder.Entity<Notification>().Property(n => n.Channel).HasConversion<string>();
        }
    }
}