namespace Tvl.VisualStudio.InheritanceMargin
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using IOutputWindowService = Tvl.VisualStudio.Shell.OutputWindow.Interfaces.IOutputWindowService;
    using TaskScheduler = System.Threading.Tasks.TaskScheduler;

    [Name("CSharp Inheritance Tagger Provider")]
    [TagType(typeof(IInheritanceTag))]
    [Export(typeof(ITaggerProvider))]
    [ContentType("CSharp")]
    public class CSharpInheritanceTaggerProvider : ITaggerProvider
    {
        public CSharpInheritanceTaggerProvider()
        {
            TaskScheduler = TaskScheduler.Default;
        }

        //[Import(PredefinedTaskSchedulers.BackgroundIntelliSense)]
        public TaskScheduler TaskScheduler
        {
            get;
            private set;
        }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService
        {
            get;
            private set;
        }

        [Import]
        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        [Import]
        public SVsServiceProvider GlobalServiceProvider
        {
            get;
            private set;
        }

        #region ITaggerProvider Members

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer != null)
                return CSharpInheritanceTagger.CreateInstance(this, buffer) as ITagger<T>;

            return null;
        }

        #endregion
    }
}
