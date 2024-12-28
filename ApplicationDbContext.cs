using Microsoft.EntityFrameworkCore;
using SeatBookingProj.Models;

namespace SeatBookingProj.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed Admins
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "Rohit Sharma", Email = "rohit@admin.com", Password = "admin123", Role = "Admin" },
                new Employee { Id = 2, Name = "Mohit Sharma", Email = "mohit@admin.com", Password = "admin123", Role = "Admin" }
            );

            // Seed Seats for CKC-Chennai
            modelBuilder.Entity<Seat>().HasData(
                Enumerable.Range(1, 90).Select(i => new Seat
                {
                    Id = i,
                    SeatNumber = i <= 60 ? $"CKC1000A{i:D3}" : $"CKC1000B{i - 60:D3}",
                    Type = i <= 60 ? "Employee" : "Guest",
                    IsBooked = false,
                    Location = "CKC-Chennai"
                }).ToArray()
            );

            // Seed Seats for Mepz-Chennai
            modelBuilder.Entity<Seat>().HasData(
                Enumerable.Range(91, 230).Select(i => new Seat
                {
                    Id = i,
                    SeatNumber = i <= 270 ? $"MEPZ1000A{i - 90:D3}" : $"MEPZ1000B{i - 270:D3}",
                    Type = i <= 270 ? "Employee" : "Guest",
                    IsBooked = false,
                    Location = "Mepz-Chennai"
                }).ToArray()
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}