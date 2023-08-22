using Aydex.SemanticKernel.NL2EF.Data;
using Aydex.SemanticKernel.NL2EF.Http;
using Aydex.SemanticKernel.NL2EF.Options;
using Aydex.SemanticKernel.NL2EF.Skills;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextEmbedding;
using Microsoft.SemanticKernel.Memory;

namespace Aydex.SemanticKernel.NL2EF.Extensions;

public static class SemanticKernelExtensions
{
    public static IServiceCollection AddSemanticKernel(this IServiceCollection services)
    {
        services.AddSingleton<IMemoryStore, VolatileMemoryStore>();
        services.AddScoped<ISemanticTextMemory>((serviceProvider) =>
        {
            var storage = serviceProvider.GetRequiredService<IMemoryStore>();
            var semanticKernelOptions = serviceProvider.GetRequiredService<IOptions<SemanticKernelOptions>>();
            var aiServiceOptions = semanticKernelOptions.Value.AIService;
            var textEmbeddingGeneration = aiServiceOptions.ToTextEmbeddingsService();
            var semanticTextMemory = new SemanticTextMemory(storage, textEmbeddingGeneration);

            return semanticTextMemory;
        });
        services.AddScoped<IKernel>((serviceProvider) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IKernel>>();
            var memory = serviceProvider.GetRequiredService<ISemanticTextMemory>();
            var semanticKernelOptions = serviceProvider.GetRequiredService<IOptions<SemanticKernelOptions>>();
            var aiServiceOptions = semanticKernelOptions.Value.AIService;
            var kernel = Kernel.Builder
                .WithLogger(logger)
                .WithMemory(memory)
                .WithRetryHandlerFactory(new RetryHandlerFactory())
                .WithEmbeddingBackend(aiServiceOptions)
                .WithCompletionBackend(aiServiceOptions)
                .Build();

            kernel.ImportSkills(serviceProvider);

            var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            var dbContextSeedTask = Task.Run(async () =>
            {
                var collections = await kernel.Memory.GetCollectionsAsync();

                if (collections.Contains(AppDbContext.SchemaMemoryCollectionName))
                {
                    return;
                }

                var dbCreateScript = dbContext.Database.GenerateCreateScript();
                var dbCreateScriptChunks = dbCreateScript.Split("\n\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                for (var i = 0; i < dbCreateScriptChunks.Length; i++)
                {
                    var chunk = dbCreateScriptChunks[i];

                    await kernel.Memory.SaveInformationAsync(AppDbContext.SchemaMemoryCollectionName, chunk, i.ToString());
                }
            });

            // These memories should be seeded as part of a preprocessing step, but for the sake of this demo...
            dbContextSeedTask.Wait();

            return kernel;
        });

        return services;
    }

    private static KernelBuilder WithCompletionBackend(this KernelBuilder kernelBuilder, AIServiceOptions options)
    {
        return options.Type switch
        {
            AIServiceOptions.AIServiceType.AzureOpenAI => kernelBuilder.WithAzureChatCompletionService(options.Models.Completion, options.Endpoint, options.Key),
            AIServiceOptions.AIServiceType.OpenAI => kernelBuilder.WithOpenAIChatCompletionService(options.Models.Completion, options.Key),
            _ => throw new ArgumentException($"Invalid {nameof(options.Type)} value."),
        };
    }

    private static KernelBuilder WithEmbeddingBackend(this KernelBuilder kernelBuilder, AIServiceOptions options)
    {
        return options.Type switch
        {
            AIServiceOptions.AIServiceType.AzureOpenAI => kernelBuilder.WithAzureTextEmbeddingGenerationService(options.Models.Embedding, options.Endpoint, options.Key),
            AIServiceOptions.AIServiceType.OpenAI => kernelBuilder.WithOpenAITextEmbeddingGenerationService(options.Models.Embedding, options.Key),
            _ => throw new ArgumentException($"Invalid {nameof(options.Type)} value."),
        };
    }

    private static ITextEmbeddingGeneration ToTextEmbeddingsService(this AIServiceOptions options)
    {
        return options.Type switch
        {
            AIServiceOptions.AIServiceType.AzureOpenAI => new AzureTextEmbeddingGeneration(options.Models.Embedding, options.Endpoint, options.Key),
            AIServiceOptions.AIServiceType.OpenAI => new OpenAITextEmbeddingGeneration(options.Models.Embedding, options.Key),
            _ => throw new ArgumentException("Invalid AIService value in embeddings backend settings"),
        };
    }

    private static void ImportSkills(this IKernel kernel, IServiceProvider serviceProvider)
    {
        var semanticKernelOptions = serviceProvider.GetRequiredService<IOptions<SemanticKernelOptions>>();
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

        if (semanticKernelOptions.Value.SemanticSkillsDirectory is string semanticSkillsDirectory)
        {
            kernel.ImportSemanticSkills(semanticSkillsDirectory);
        }

        kernel.ImportNL2EFSkill(dbContext);
    }

    private static void ImportSemanticSkills(this IKernel kernel, string skillsDirectory)
    {
        foreach (var subDirectory in Directory.GetDirectories(skillsDirectory))
        {
            try
            {
                kernel.ImportSemanticSkillFromDirectory(skillsDirectory, Path.GetFileName(subDirectory));
            }
            catch (Exception e)
            {
                kernel.Logger.LogError("Failed to import skill from {Directory} ({Message})", subDirectory, e.Message);
            }
        }
    }

    private static void ImportNL2EFSkill(this IKernel kernel, AppDbContext dbContext)
    {
        var skill = new NL2EFSkill(kernel, dbContext);

        kernel.ImportSkill(skill, nameof(NL2EFSkill));
    }
}
