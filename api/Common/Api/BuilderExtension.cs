using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Common.Api
{
    public static class BuilderExtension
    {
        public static void AddConfiguration(this WebApplicationBuilder builder)
        {
            Configuration.ConnectionString = builder.Configuration.GetValue<string>("conexao") ?? string.Empty;

        }

        public static void AddDocumentation(this WebApplicationBuilder builder)
        {

            // builder.Services.AddEndpointsApiExplorer();
            // builder.Services.AddSwaggerGen(x =>
            // {
            //     x.CustomSchemaIds(x => x.FullName);
            // });

            
        }

        public static void AddSecurity(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentityApiEndpoints<IdentityUser>();
                            //.AddEntityFrameworkStores<ApplicationDbContext>();

            // builder.Services.Configure<IdentityOptions>(options =>
            // {
            //     options.SignIn.RequireConfirmedEmail = true;
            //     options.Lockout.MaxFailedAccessAttempts = 20;

            // });

            // builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
            //                 .AddIdentityCookies();
            // builder.Services.ConfigureApplicationCookie(options =>
            // {
            //     options.Cookie.Name = "Investment.AuthCookie";
            //     options.Cookie.HttpOnly = true;
            //     options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            //     options.Cookie.SameSite = SameSiteMode.None;
            //     options.Cookie.IsEssential = true;
            //     options.ExpireTimeSpan = TimeSpan.FromDays(7);
            //     options.SlidingExpiration = true;
            // });

            builder.Services.AddAuthorization();
        }

        public static void AddDataContexts(this WebApplicationBuilder builder)
        {

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(Configuration.ConnectionString);
            });

            builder.Services.AddIdentityCore<IdentityUser>()
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddApiEndpoints()
                            ;
        }


        public static void AddServices(this WebApplicationBuilder builder)
        {

        }
    }
}