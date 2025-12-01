using api.Common.Api;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.AddConfiguration();
builder.AddSecurity();
builder.AddDataContexts();
builder.AddDocumentation();
builder.AddServices();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

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
