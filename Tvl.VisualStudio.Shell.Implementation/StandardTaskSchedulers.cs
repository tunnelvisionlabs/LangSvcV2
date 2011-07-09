namespace Tvl.VisualStudio.Shell.Implementation
{
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using Tvl.VisualStudio.Shell.OutputWindow;

    public class StandardTaskSchedulers
    {
        [Export(PredefinedTaskSchedulers.BackgroundIntelliSense, typeof(TaskScheduler))]
        private readonly BackgroundParserTaskScheduler BackgroundIntelliSenseScheduler;

        [Export(PredefinedTaskSchedulers.PriorityIntelliSense, typeof(TaskScheduler))]
        private readonly BackgroundParserTaskScheduler PriorityIntelliSenseScheduler;

        [ImportingConstructor]
        public StandardTaskSchedulers(IOutputWindowService outputWindowService)
        {
            BackgroundIntelliSenseScheduler = new BackgroundParserTaskScheduler(outputWindowService);
            PriorityIntelliSenseScheduler = new BackgroundParserTaskScheduler("TVL Priority IntelliSense", 2, outputWindowService);
        }
    }
}
