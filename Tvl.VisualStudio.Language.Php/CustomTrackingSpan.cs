/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * This source code has been modified from its original form.
 *
 * ***************************************************************************/

namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// This tracking span wraps an underlying <see cref="ITrackingSpan"/> but treating it as though
    /// the <see cref="TrackingMode"/> is <see cref="SpanTrackingMode.Custom"/>. All calls other than
    /// the <see cref="TrackingMode"/> property are delegated to the underlying <see cref="ITrackingSpan"/>.
    /// </summary>
    internal class CustomTrackingSpan : ITrackingSpan
    {
        private readonly ITrackingSpan _referenceSpan;

        public CustomTrackingSpan(ITrackingSpan referenceSpan)
        {
            if (referenceSpan == null)
                throw new ArgumentNullException("referenceSpan");

            _referenceSpan = referenceSpan;
        }

        #region ITrackingSpan Members

        public SnapshotPoint GetEndPoint(ITextSnapshot snapshot)
        {
            return _referenceSpan.GetEndPoint(snapshot);
        }

        public Span GetSpan(ITextVersion version)
        {
            return _referenceSpan.GetSpan(version);
        }

        public SnapshotSpan GetSpan(ITextSnapshot snapshot)
        {
            return _referenceSpan.GetSpan(snapshot);
        }

        public SnapshotPoint GetStartPoint(ITextSnapshot snapshot)
        {
            return _referenceSpan.GetStartPoint(snapshot);
        }

        public string GetText(ITextSnapshot snapshot)
        {
            return _referenceSpan.GetText(snapshot);
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return _referenceSpan.TextBuffer;
            }
        }

        public TrackingFidelityMode TrackingFidelity
        {
            get
            {
                return _referenceSpan.TrackingFidelity;
            }
        }

        public SpanTrackingMode TrackingMode
        {
            get
            {
                return SpanTrackingMode.Custom;
            }
        }

        #endregion
    }
}
