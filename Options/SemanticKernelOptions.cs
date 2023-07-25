namespace Aydex.SemanticKernel.NL2EF.Options;

public class SemanticKernelOptions
{
    public const string SemanticKernel = "SemanticKernel";

    public string SemanticSkillsDirectory { get; set; } = default!;
    public string OpenAIApiKey { get; set; } = default!;
    public string OpenAIChatCompletionModel { get; set; } = default!;
    public string OpenAITextEmbeddingModel { get; set; } = default!;
}
