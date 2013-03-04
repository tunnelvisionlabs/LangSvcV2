namespace Tvl.VisualStudio.Language.Php.Classification
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Projection;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Classification;
    using Path = System.IO.Path;
    using StringComparison = System.StringComparison;

    [Export(typeof(IClassifierProvider))]
    [ContentType(PhpConstants.PhpContentType)]
    public sealed class PhpClassifierProvider : LanguageClassifierProvider<PhpLanguagePackage>
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService
        {
            get;
            private set;
        }

        [Import]
        public IBufferGraphFactoryService BufferGraphFactoryService
        {
            get;
            private set;
        }

        protected override IClassifier GetClassifierImpl(ITextBuffer textBuffer)
        {
            if (!textBuffer.ContentType.IsOfType(PhpConstants.PhpContentType))
                return null;

            IBufferGraph graph = BufferGraphFactoryService.CreateBufferGraph(textBuffer);
            var rootBuffers = graph.GetTextBuffers(buffer => !(buffer is IProjectionBufferBase));

            ITextDocument textDocument;
            if (TextDocumentFactoryService.TryGetTextDocument(rootBuffers.FirstOrDefault() ?? textBuffer, out textDocument))
            {
                if (!string.IsNullOrEmpty(textDocument.FilePath)
                    && Path.GetExtension(textDocument.FilePath).Equals(PhpConstants.Php5FileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    return new V4PhpClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
                }
            }

            return new PhpClassifier(textBuffer, StandardClassificationService, ClassificationTypeRegistryService);
        }
    }
}
