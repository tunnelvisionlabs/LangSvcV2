namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;

    /// <summary>
    /// Classifier for projection buffers - used simply to raise that the tags have
    /// changed in the overall buffer.
    /// </summary>
    internal class ProjectionClassifier : IClassifier
    {
        #region IClassifier Members

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            return new ClassificationSpan[0];
        }

        #endregion

        internal void RaiseClassificationChanged(SnapshotPoint start, SnapshotPoint end)
        {
            var classChanged = ClassificationChanged;
            if (classChanged != null)
            {
                classChanged(this, new ClassificationChangedEventArgs(new SnapshotSpan(start, end)));
            }
        }
    }
}
