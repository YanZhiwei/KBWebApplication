using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Memory;
using Tenon.AspNetCore.Controllers;

namespace KBWebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class knowledgeBaseController : AbstractController
{
    private readonly Kernel _kernel;
    private readonly ILogger<knowledgeBaseController> _logger;
    private readonly string _prompt;

    public knowledgeBaseController(Kernel kernel, ILogger<knowledgeBaseController> logger)
    {
        _kernel = kernel;
        _logger = logger;
        _prompt = System.IO.File.ReadAllText(@"Prompts\skprompt.txt");
    }

    [HttpPost(Name = "ask")]
    [Experimental("SKEXP0001")]
    public async Task<ActionResult<string>> AskAsync([FromBody] string query)
    {
        _kernel.CreateFunctionFromPrompt(_prompt, functionName: "QuerySkill");
        var result = await _kernel.InvokeAsync(_kernel.Plugins.GetFunction(nameof(TextMemoryPlugin), "QuerySkill"),
            new KernelArguments {{"input", query}});
        return Result(result.GetValue<string>());
    }
}