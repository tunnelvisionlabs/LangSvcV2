namespace Tvl.VisualStudio.Text.Classification
{
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Language.StandardClassification;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Tvl.VisualStudio.Shell;

    public abstract class LanguageClassifierProvider<TLanguagePackage> : IClassifierProvider
        where TLanguagePackage : Package
    {
        private static bool _languagePackageLoaded;

        [Import]
        public IStandardClassificationService StandardClassificationService
        {
            get;
            private set;
        }

        [Import]
        public IClassificationTypeRegistryService ClassificationTypeRegistryService
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

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                return null;

            if (!_languagePackageLoaded)
            {
                var languagePackage = GlobalServiceProvider.GetShell().LoadPackage<TLanguagePackage>();
                _languagePackageLoaded = languagePackage != null;
            }

            return GetClassifierImpl(textBuffer);
        }

        protected abstract IClassifier GetClassifierImpl([NotNull] ITextBuffer textBuffer);
    }
}
