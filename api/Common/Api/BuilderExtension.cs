using System.Reflection;
using application.Services;
using core.Repositories;
using core.Services;
using infra.Data;
using infra.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Investment API",
                    Description = "Web API for investments",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Investment API Contact",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Investment API License",
                        Url = new Uri("https://example.com/license")
                    }
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });


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
            builder.Services.AddScoped<IInvestmentRepository, InvestmentRepository>();

            builder.Services.AddScoped<IInvestmentService, InvestmentService>();
            builder.Services.AddScoped<IGainCalculationService, GainCalculationService>();
            builder.Services.AddScoped<ITaxCalculationService, TaxCalculationService>();
        }
    }
}