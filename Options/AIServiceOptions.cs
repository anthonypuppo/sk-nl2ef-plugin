namespace Aydex.SemanticKernel.NL2EF.Options;

public sealed class AIServiceOptions
{
    public const string AIService = "AIService";

    public enum AIServiceType
    {
        OpenAI = 1,
        AzureOpenAI
    }

    public class ModelTypes
    {
        public string Completion { get; set; } = string.Empty;

        public string Embedding { get; set; } = string.Empty;
    }

    public AIServiceType Type { get; set; } = AIServiceType.OpenAI;

    public ModelTypes Models { get; set; } = new ModelTypes();

    public string Endpoint { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;
}
