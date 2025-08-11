using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using WebUseASP_test_.Models;
using Microsoft.EntityFrameworkCore;
using WebUseASP_test_.Data;


namespace WebUseASP_test_.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<Authentication> Authentications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
