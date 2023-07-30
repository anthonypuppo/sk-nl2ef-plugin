namespace Aydex.SemanticKernel.NL2EF.Options;

public class SemanticKernelOptions
{
    public const string SemanticKernel = "SemanticKernel";

    public string SemanticSkillsDirectory { get; set; } = default!;
    public AIServiceOptions AIService { get; set; } = default!;
}
