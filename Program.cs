using AnthonyPuppo.SemanticKernel.NL2EF.Data;
using AnthonyPuppo.SemanticKernel.NL2EF.Extensions;
using AnthonyPuppo.SemanticKernel.NL2EF.Options;
using AnthonyPuppo.SemanticKernel.NL2EF.Skills;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors((options) =>
{
    options.AddDefaultPolicy((policy) =>
    {
        policy
            .WithOrigins("https://chat.openai.com", "https://localhost:7012")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddOptions<SemanticKernelOptions>()
    .Bind(builder.Configuration.GetSection(SemanticKernelOptions.SemanticKernel));

builder.Services.AddDbContext<AppDbContext>((options) =>
{
    options.UseSqlite("Data Source=movies.sqlite;Mode=ReadOnly");
});

builder.Services.AddSemanticKernel();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen((options) =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "NL2EF",
        Version = "v1",
        Description = "This is a sample API for NL2EF",
    });
});

var app = builder.Build();

app.UseCors();

app.UseSwagger((options) =>
{
    options.PreSerializeFilters.Add((swaggerDocument, httpRequest) =>
    {
        swaggerDocument.Servers = new List<OpenApiServer>()
        {
            new OpenApiServer()
            {
                Url = $"{httpRequest.Scheme}://{httpRequest.Host.Value}"
            }
        };
    });
});
app.UseSwaggerUI((options) =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "NL2EF v1");
});

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), ".well-known")),
    RequestPath = "/.well-known"
});

app
    .MapGet("/query", async ([FromServices] IKernel kernel, string question) =>
    {
        var context = kernel.CreateNewContext();
        var function = kernel.Skills.GetFunction(nameof(NL2EFSkill), nameof(NL2EFSkill.Query));

        context.Variables.Update(question);
        context = await function.InvokeAsync(context);

        return context.Result;
    })
    .WithName("Query")
    .WithOpenApi((operation) =>
    {
        operation.Description = "Translates a question into SQL, fetches relevant data from the database, and formulates a response based on the retrieved data.";

        var parameter = operation.Parameters[0];

        parameter.Description = "The question to query the database for";

        return operation;
    });

app.Run();
