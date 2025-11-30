using api.Common.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.AddConfiguration();
builder.AddSecurity();
builder.AddDataContexts();
builder.AddDocumentation();
builder.AddServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minha API",
        Version = "v1"
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
});


app.UseHttpsRedirection();

app.MapGroup("/api/Identity")
    .WithTags("Identity")
    .MapIdentityApi<IdentityUser>();

app.MapControllers();

app.Run();
