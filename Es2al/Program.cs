
using Es2al.DataAccess.Context;
using Es2al.DataAccess.Repositories;
using Es2al.DataAccess.Repositories.IRepositroies;
using Es2al.Filters;
using Es2al.Models.Entites;
using Es2al.Services;
using Es2al.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Es2al
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new CustomeErrorHandlerAttribute());
            });
            builder.Services.AddDbContext<AppDbContext>(e => {
                e.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
                e.UseSqlServer(builder.Configuration.GetConnectionString("constr"),
                options => {
                    options.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);

                });

            });
        
            
            builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddUserManager<ApplicationUserService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
            builder.Services.AddScoped<IUserFollowRepository,UserFollowRepository>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<IQuestionTagRepository, QuestionTagRepository>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IFollowingService, FollowingService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IQuestionThreadsService, QuestionThreadsService>();
            builder.Services.AddScoped<IAnswerService, AnswerService>();
            builder.Services.AddScoped<IReactionService, ReactionService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IQuestionTagService, QuestionTagService>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";  //set the login page.  
            });
           
            var app = builder.Build();
     
            using (var scope = app.Services.CreateScope())
            {
                await CreateRolesAsync(scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Feed}/{action=Index}");

            app.Run();
        }
        private static async Task CreateRolesAsync(IServiceProvider serviceProvider)
        {
       
            var userManager = serviceProvider.GetRequiredService<ApplicationUserService>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                   await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }
            var user = await userManager.FindByEmailAsync("admin@admin.com");
            if(user == null)
            {
                var admin = new AppUser() { UserName = "Admin", Email = "admin@admin.com" };
                string adminPassword = "Admin0";
                var createAdmin = await userManager.CreateAsync(admin, adminPassword);
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
