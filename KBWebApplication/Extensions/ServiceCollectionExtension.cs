using System.Diagnostics.CodeAnalysis;
using KBWebApplication.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;

namespace KBWebApplication.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSemanticKernel(this IServiceCollection services,
        IConfigurationRoot configBuilder)
    {
        var completionOptions = configBuilder.GetSection("Completion").Get<AzureOpenAiOptions>();
        if (completionOptions == null)
            throw new NullReferenceException(nameof(completionOptions));
        var builder = Kernel.CreateBuilder();
        builder.AddAzureOpenAIChatCompletion(
            completionOptions.DeploymentId,
            completionOptions.Endpoint, // Azure OpenAI Endpoint
            completionOptions.Key); // Azure OpenAI Key
        var kernel = builder.Build();
        services.AddSingleton(kernel);
        return services;
    }

    [Experimental("SKEXP0001")]
    public static IServiceCollection AddSemanticTextMemory(this IServiceCollection services,
        IConfigurationRoot configBuilder)
    {
        var embeddingOptions = configBuilder.GetSection("Embedding").Get<AzureOpenAiOptions>();
        if (embeddingOptions == null)
            throw new NullReferenceException(nameof(embeddingOptions));
        var memoryBuilder = new MemoryBuilder();
        memoryBuilder.WithAzureOpenAITextEmbeddingGeneration(
            embeddingOptions.DeploymentId, embeddingOptions.Endpoint,
            embeddingOptions.Key);
        memoryBuilder.WithMemoryStore(new VolatileMemoryStore());
        var memory = memoryBuilder.Build();
        services.AddSingleton(memory);
        return services;
    }
}