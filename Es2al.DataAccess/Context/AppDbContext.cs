using Es2al.Models.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Es2al.DataAccess.Context
{
    public class AppDbContext:IdentityDbContext<AppUser,IdentityRole<int>,int>
    {
        public DbSet<Tag> Tag { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<UserFollow> UserFollow { get; set; }
        public DbSet<QuestionTag> QuestionTag { get; set; }
        public DbSet<UserTag> UserTag { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(builder);
        }

    }
}
