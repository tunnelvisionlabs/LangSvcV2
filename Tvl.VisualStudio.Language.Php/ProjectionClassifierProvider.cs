namespace Tvl.VisualStudio.Language.Php
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// Provides a classifier for our HTML projection buffers so we can raise that the tags
    /// have changed on the full buffer when we switch between portions being HTML and portions
    /// being Django tags.
    /// </summary>
    [Export(typeof(IClassifierProvider)), ContentType("projection")]
    internal class ProjectionClassifierProvider : IClassifierProvider
    {
        #region IClassifierProvider Members

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            ProjectionClassifier res;
            if (!textBuffer.Properties.TryGetProperty<ProjectionClassifier>(typeof(ProjectionClassifier), out res) &&
                textBuffer.Properties.ContainsProperty(typeof(PhpProjectionBuffer)))
            {
                res = new ProjectionClassifier();
                textBuffer.Properties.AddProperty(typeof(ProjectionClassifier), res);
            }
            return res;
        }

        #endregion
    }
}
