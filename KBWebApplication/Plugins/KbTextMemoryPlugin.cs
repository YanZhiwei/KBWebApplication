using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace KBWebApplication.Plugins;

public class KbTextMemoryPlugin
{
    private readonly Kernel _kernel;
    [Experimental("SKEXP0001")] private readonly ISemanticTextMemory _memory;

    [Experimental("SKEXP0001")]
    public KbTextMemoryPlugin(Kernel kernel, ISemanticTextMemory memory)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _memory = memory ?? throw new ArgumentNullException(nameof(memory));
    }

    [Experimental("SKEXP0050")]
    public async Task ImportAsync(string[] pdfFiles)
    {
        const int maxContentItemSize = 2048;
        if (pdfFiles?.Any() ?? false) return;
        foreach (var pdfFileName in pdfFiles)
        {
            using var pdfDocument = PdfDocument.Open(pdfFileName);
            foreach (var pdfPage in pdfDocument.GetPages())
            {
                var pageText = ContentOrderTextExtractor.GetText(pdfPage);

                var paragraphs = new List<string>();


                if (pageText.Length > maxContentItemSize)
                {
                    var lines = TextChunker.SplitPlainTextLines(pageText, maxContentItemSize);
                    paragraphs = TextChunker.SplitPlainTextParagraphs(lines, maxContentItemSize);
                }
                else
                {
                    paragraphs.Add(pageText);
                }


                foreach (var paragraph in paragraphs)
                {
                    var id = pdfFileName + pdfPage.Number + paragraphs.IndexOf(paragraph);

                    await _memory.SaveInformationAsync("kb-memory-collection", paragraph, id);
                }
            }
        }

        _kernel.ImportPluginFromObject(new TextMemoryPlugin(_memory), nameof(TextMemoryPlugin));
    }
}