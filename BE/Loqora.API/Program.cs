using Microsoft.EntityFrameworkCore;
using Loqora.Application;
using Loqora.Infrastructure;
using Loqora.Infrastructure.Data;
using Loqora.Web;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Loqora API V1");

        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.EnableFilter();
    });

    app.MapScalarApiReference();


    await app.InitialiseDatabaseAsync();

}
else
{
    app.UseHsts();
}

app.UseCoreMiddlewares(builder.Configuration);

app.MapControllers();
app.MapPrometheusScrapingEndpoint();

//app.UseAntiforgery();

app.MapStaticAssets();



using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();
}

app.Run();
