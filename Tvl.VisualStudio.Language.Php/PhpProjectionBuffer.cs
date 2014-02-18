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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Differencing;
    using Microsoft.VisualStudio.Text.Projection;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Language.Php.Projection;
    using Debug = System.Diagnostics.Debug;

    internal class PhpProjectionBuffer : IProjectionEditResolver
    {
        private static readonly EditOptions _editOptions = new EditOptions(true, new StringDifferenceOptions(StringDifferenceTypes.Character, 0, false));

        private readonly ITextBuffer _diskBuffer;           // the buffer as it appears on disk.
        private readonly IProjectionBuffer _projBuffer; // the buffer we project into        
        private readonly List<SpanInfo> _spans = new List<SpanInfo>();
        private readonly IContentTypeRegistryService _contentRegistry;
        private readonly IBufferGraph _bufferGraph;
        private readonly IContentType _contentType;
        private readonly IElisionBuffer _htmlBuffer;
        private readonly IProjectionBuffer _templateBuffer;

        private readonly ITagAggregator<ContentTypeTag> _contentTypeTagger;

        public PhpProjectionBuffer(
            IContentTypeRegistryService contentRegistry,
            IProjectionBufferFactoryService bufferFactory,
            IBufferTagAggregatorFactoryService bufferTagAggregatorFactory,
            ITextBuffer diskBuffer,
            IBufferGraphFactoryService bufferGraphFactory,
            IContentType contentType)
        {
            _diskBuffer = diskBuffer;
            _contentRegistry = contentRegistry;
            _contentType = contentType;

            _projBuffer = CreateProjectionBuffer(bufferFactory);
            _htmlBuffer = CreateHtmlBuffer(bufferFactory);
            _templateBuffer = CreateTemplateBuffer(bufferFactory);

            _bufferGraph = bufferGraphFactory.CreateBufferGraph(_projBuffer);

            _contentTypeTagger = bufferTagAggregatorFactory.CreateTagAggregator<ContentTypeTag>(_diskBuffer);
            _contentTypeTagger.TagsChanged += HandleContentTypeTagsChanged;

            IVsTextBuffer buffer;
            if (_diskBuffer.Properties.TryGetProperty<IVsTextBuffer>(typeof(IVsTextBuffer), out buffer))
            {
                // keep the Venus HTML classifier happy - it wants to find a site via IVsTextBuffer
                _htmlBuffer.Properties.AddProperty(typeof(IVsTextBuffer), buffer);
            }

            HandleContentTypeTagsChanged();
        }

        public List<SpanInfo> Spans
        {
            get
            {
                return _spans;
            }
        }

        private void HandleContentTypeTagsChanged(object sender, TagsChangedEventArgs e)
        {
            HandleContentTypeTagsChanged();
        }

        private void HandleContentTypeTagsChanged()
        {
            ITextSnapshot snapshot = DiskBuffer.CurrentSnapshot;
            ITextSnapshot htmlSnapshot = _htmlBuffer.CurrentSnapshot;
            ITextSnapshot phpSnapshot = TemplateBuffer.CurrentSnapshot;

            SnapshotSpan completeSnapshot = new SnapshotSpan(snapshot, 0, snapshot.Length);

            IMappingTagSpan<ContentTypeTag>[] tags = _contentTypeTagger.GetTags(completeSnapshot).ToArray();

            IMappingPoint[] trackingPoints = new IMappingPoint[tags.Length];
            for (int i = 0; i < tags.Length; i++)
            {
                IMappingTagSpan<ContentTypeTag> tag = tags[i];
                switch (tag.Tag.RegionType)
                {
                case RegionType.Begin:
                    trackingPoints[i] = tag.Span.Start;
                    break;

                case RegionType.End:
                    trackingPoints[i] = tag.Span.End;
                    break;

                default:
                    throw new NotSupportedException();
                }
            }

            List<ITrackingSpan> projectionSpans = new List<ITrackingSpan>();
            List<SpanInfo> spans = new List<SpanInfo>();
            for (int i = 0; i < tags.Length; i++)
            {
                IMappingTagSpan<ContentTypeTag> tag = tags[i];

                Span sourceSpan;
                if (i == 0)
                {
                    sourceSpan = new Span(0, trackingPoints[0].GetPoint(snapshot, PositionAffinity.Successor) ?? 0);
                }
                else
                {
                    SnapshotPoint? startPoint = trackingPoints[i - 1].GetPoint(snapshot, PositionAffinity.Successor);
                    SnapshotPoint? endPoint = trackingPoints[i].GetPoint(snapshot, PositionAffinity.Successor);
                    if (!startPoint.HasValue || !endPoint.HasValue)
                        throw new InvalidOperationException();

                    sourceSpan = Span.FromBounds(startPoint.Value.Position, endPoint.Value.Position);
                }

                switch (tag.Tag.RegionType)
                {
                case RegionType.Begin:
                    {
                        // handle the Text region that ended where this tag started
                        ITrackingSpan projectionSpan = htmlSnapshot.Version.CreateCustomTrackingSpan(
                            sourceSpan,
                            TrackingFidelityMode.Forward,
                            new LanguageSpanCustomState(sourceSpan),
                            TrackToVersion);
                        ITrackingSpan diskBufferSpan = snapshot.Version.CreateCustomTrackingSpan(
                            sourceSpan,
                            TrackingFidelityMode.Forward,
                            new LanguageSpanCustomState(sourceSpan),
                            TrackToVersion);

                        projectionSpans.Add(projectionSpan);
                        spans.Add(new SpanInfo(diskBufferSpan, TemplateTokenKind.Text));
                        break;
                    }

                case RegionType.End:
                    {
                        // handle the code region that ended where this tag ended
                        ITrackingSpan projectionSpan = new CustomTrackingSpan(phpSnapshot.CreateTrackingSpan(sourceSpan, SpanTrackingMode.EdgeExclusive));
                        ITrackingSpan diskBufferSpan = new CustomTrackingSpan(snapshot.CreateTrackingSpan(sourceSpan, SpanTrackingMode.EdgeExclusive));

                        projectionSpans.Add(projectionSpan);
                        spans.Add(new SpanInfo(diskBufferSpan, TemplateTokenKind.Block));
                        break;
                    }

                default:
                    throw new NotSupportedException();
                }
            }

            if (true)
            {
                int startPosition = 0;
                if (tags.Length > 0)
                {
                    SnapshotPoint? startPoint = trackingPoints.Last().GetPoint(snapshot, PositionAffinity.Successor);
                    if (!startPoint.HasValue)
                        throw new InvalidOperationException();

                    startPosition = startPoint.Value.Position;
                }

                Span sourceSpan = Span.FromBounds(startPosition, snapshot.Length);

                RegionType finalRegionType = tags.Length > 0 ? tags.Last().Tag.RegionType : RegionType.End;
                switch (finalRegionType)
                {
                case RegionType.Begin:
                    {
                        // handle the code region that ended at the end of the document
                        ITrackingSpan projectionSpan = new CustomTrackingSpan(phpSnapshot.CreateTrackingSpan(sourceSpan, SpanTrackingMode.EdgePositive));
                        ITrackingSpan diskBufferSpan = new CustomTrackingSpan(snapshot.CreateTrackingSpan(sourceSpan, SpanTrackingMode.EdgePositive));

                        projectionSpans.Add(projectionSpan);
                        spans.Add(new SpanInfo(diskBufferSpan, TemplateTokenKind.Block));
                        break;
                    }

                case RegionType.End:
                    {
                        // handle the Text region that ended at the end of the document
                        ITrackingSpan projectionSpan = htmlSnapshot.Version.CreateCustomTrackingSpan(
                            sourceSpan,
                            TrackingFidelityMode.Forward,
                            new LanguageSpanCustomState(sourceSpan),
                            TrackToVersion);
                        ITrackingSpan diskBufferSpan = snapshot.Version.CreateCustomTrackingSpan(
                            sourceSpan,
                            TrackingFidelityMode.Forward,
                            new LanguageSpanCustomState(sourceSpan),
                            TrackToVersion);

                        projectionSpans.Add(projectionSpan);
                        spans.Add(new SpanInfo(diskBufferSpan, TemplateTokenKind.Text));
                        break;
                    }

                default:
                    throw new NotSupportedException();
                }
            }

            int startSpan = 0;
            int oldSpanCount = _spans.Count;
            _spans.RemoveRange(startSpan, oldSpanCount - startSpan);
            _spans.AddRange(spans);

            _projBuffer.ReplaceSpans(startSpan, oldSpanCount - startSpan, projectionSpans.ToArray(), _editOptions, null);

            ProjectionClassifier classifier;
            if (spans.Count > 0 &&
                _projBuffer.Properties.TryGetProperty<ProjectionClassifier>(typeof(ProjectionClassifier), out classifier))
            {
                classifier.RaiseClassificationChanged(
                    spans[0].DiskBufferSpan.GetStartPoint(_diskBuffer.CurrentSnapshot),
                    spans[spans.Count - 1].DiskBufferSpan.GetEndPoint(_diskBuffer.CurrentSnapshot)
                );
            }
        }

        private IProjectionBuffer CreateProjectionBuffer(IProjectionBufferFactoryService bufferFactory)
        {
            var res = bufferFactory.CreateProjectionBuffer(this, new object[0], ProjectionBufferOptions.None);
            res.Properties.AddProperty(typeof(PhpProjectionBuffer), this);
            return res;
        }

        private IProjectionBuffer CreateTemplateBuffer(IProjectionBufferFactoryService bufferFactory)
        {
            var res = bufferFactory.CreateProjectionBuffer(
                this,
                new object[] { 
                    _diskBuffer.CurrentSnapshot.CreateTrackingSpan(
                        0,
                        _diskBuffer.CurrentSnapshot.Length,
                        SpanTrackingMode.EdgeInclusive,
                        TrackingFidelityMode.Forward
                    )
                },
                ProjectionBufferOptions.None,
                _contentRegistry.GetContentType(PhpConstants.PhpContentType)
            );
            res.Properties.AddProperty(typeof(PhpProjectionBuffer), this);
            return res;
        }

        private IElisionBuffer CreateHtmlBuffer(IProjectionBufferFactoryService bufferFactory)
        {
            var res = bufferFactory.CreateElisionBuffer(
                this,
                new NormalizedSnapshotSpanCollection(
                    new SnapshotSpan(
                        _diskBuffer.CurrentSnapshot,
                        new Span(0, _diskBuffer.CurrentSnapshot.Length)
                    )
                ),
                ElisionBufferOptions.None,
                _contentType
            );
            res.Properties.AddProperty(typeof(PhpProjectionBuffer), this);
            return res;
        }

        public IProjectionBuffer ProjectionBuffer
        {
            get
            {
                return _projBuffer;
            }
        }

        public IProjectionBuffer TemplateBuffer
        {
            get
            {
                return _templateBuffer;
            }
        }

        public IBufferGraph BufferGraph
        {
            get
            {
                return _bufferGraph;
            }
        }

        public ITextBuffer DiskBuffer
        {
            get
            {
                return _diskBuffer;
            }
        }

        private class LanguageSpanCustomState
        {
            public int start;       // current start of span
            public int end;         // current end of span
            public bool inelastic;  // when a span becomes inelastic, it will no longer grow due

            // to insertions at its edges
            public LanguageSpanCustomState(Span span)
            {
                this.start = span.Start;
                this.end = span.End;
            }
        }

        // Taken from Venus HTML editor as-is
        public static Span TrackToVersion(ITrackingSpan customSpan, ITextVersion currentVersion, ITextVersion targetVersion, Span currentSpan, object customState)
        {
            // We want insertions at the edges of the span to cause it to grow (characters typed at the edge of a nugget or script block
            // should go inside that nugget or block). However, if a replacement overlaps the span but also deletes text outside the span,
            // we do not want the span to grow to include the new text. We will wait for the CBM to reparse the text to determine which parts
            // belong to the embedded language. We need to be conservative, because the embedded language compiler (e.g. VB) thinks it is free
            // to change the text inside the nugget, which is inappropriate if it is really just markup.
            // Once text at the boundary of a span has been deleted, the span can no longer grow on subsequent insertions -- further grow must wait
            // for reparsing.
            LanguageSpanCustomState state = (LanguageSpanCustomState)customState;
            ITextVersion v = currentVersion;
            int start = state.start;
            int end = state.end;
            Span current = Span.FromBounds(start, end);
            if (targetVersion.VersionNumber > currentVersion.VersionNumber)
            {
                // map forward in time
                while (v != targetVersion)
                {
                    int changeCount = v.Changes.Count;
                    for (int c = 0; c < changeCount; ++c)
                    {
                        ITextChange textChange = v.Changes[c];
                        Span deletedSpan = new Span(textChange.NewPosition, textChange.OldLength);

                        if (deletedSpan.End < start)
                        {
                            // the whole thing is before our span. shift.
                            start += textChange.Delta;
                            end += textChange.Delta;
                        }
                        else if (current.Contains(deletedSpan))
                        {
                            // Our span subsumes the whole thing. shrink or grow, unless
                            // we are dead and this is an insertion at a boundary
                            if (state.inelastic && deletedSpan.Length == 0 && textChange.NewPosition == start)
                            {
                                start += textChange.Delta;
                                end += textChange.Delta;
                            }
                            else if (state.inelastic && deletedSpan.Length == 0 && textChange.NewPosition == end)
                            {
                                break;
                            }
                            else
                            {
                                end += textChange.Delta;
                            }
                        }
                        else if (end <= textChange.NewPosition)
                        {
                            // the whole thing is to our right - no impact on us.
                            // since changes are sorted, we are done with this version.
                            break;
                        }
                        else
                        {
                            // there is overlap of the OldSpan and our span, but it is not
                            // a subset. We don't want to include any new text. 
                            // this span can never again absorb an insertion at its boundary
                            state.inelastic = true;
                            if (deletedSpan.Start <= start && deletedSpan.End >= end)
                            {
                                // a proper superset of our span was deleted (we already handled
                                // the case where exactly our span was deleted).
                                start = textChange.NewEnd;
                                end = textChange.NewEnd;
                            }
                            else if (deletedSpan.End < end)
                            {
                                // the deletion overlaps start but not end. 
                                start = textChange.NewEnd;
                                end += textChange.Delta;
                            }
                            else
                            {
                                // the deletion overlaps end but not start.
                                // start doesn't change
                                end = textChange.NewPosition;
                            }
                        }
                        current = Span.FromBounds(start, end);
                    }
                    v = v.Next;
                }
            }
            else
            {
                Debug.Fail("Mapping language span backward in time!");
                // map backwards. we don't claim to do anything useful.
                return current;
            }
            state.start = start;
            state.end = end;
            return current;
        }

        internal struct SpanInfo
        {
            public readonly ITrackingSpan DiskBufferSpan;
            public readonly TemplateTokenKind Kind;

            public SpanInfo(ITrackingSpan diskBufferSpan, TemplateTokenKind kind)
            {
                DiskBufferSpan = diskBufferSpan;
                Kind = kind;
            }
        }

        private class ComparisonTrackingSpan : ITrackingSpan
        {
            private readonly int _start, _end;
            public ComparisonTrackingSpan(int start, int end)
            {
                _start = start;
                _end = end;
            }

            #region ITrackingSpan Members

            public SnapshotPoint GetEndPoint(ITextSnapshot snapshot)
            {
                throw new NotImplementedException();
            }

            public Span GetSpan(ITextVersion version)
            {
                return Span.FromBounds(_start, _end);
            }

            public SnapshotSpan GetSpan(ITextSnapshot snapshot)
            {
                throw new NotImplementedException();
            }

            public SnapshotPoint GetStartPoint(ITextSnapshot snapshot)
            {
                throw new NotImplementedException();
            }

            public string GetText(ITextSnapshot snapshot)
            {
                throw new NotImplementedException();
            }

            public ITextBuffer TextBuffer
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public TrackingFidelityMode TrackingFidelity
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public SpanTrackingMode TrackingMode
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            #endregion;
        }

        private class TrackingSpanComparer : IComparer<SpanInfo>
        {
            private readonly ITextSnapshot _snapshot;

            public TrackingSpanComparer(ITextSnapshot snapshot)
            {
                _snapshot = snapshot;
            }

            #region IComparer<ITrackingSpan> Members

            public int Compare(SpanInfo x, SpanInfo y)
            {
                var xSpan = x.DiskBufferSpan;
                var ySpan = y.DiskBufferSpan;

                return xSpan.GetSpan(_snapshot.Version).Start - ySpan.GetSpan(_snapshot.Version).Start;
            }

            #endregion
        }

        #region IProjectionEditResolver Members

        public void FillInInsertionSizes(SnapshotPoint projectionInsertionPoint, ReadOnlyCollection<SnapshotPoint> sourceInsertionPoints, string insertionText, IList<int> insertionSizes)
        {
            for (int i = 0; i < sourceInsertionPoints.Count; i++)
            {
                if (sourceInsertionPoints[i].Snapshot.TextBuffer == _htmlBuffer)
                {
                    insertionSizes[i] += insertionText.Length;
                    break;
                }
            }
        }

        public void FillInReplacementSizes(SnapshotSpan projectionReplacementSpan, ReadOnlyCollection<SnapshotSpan> sourceReplacementSpans, string insertionText, IList<int> insertionSizes)
        {
            for (int i = 0; i < sourceReplacementSpans.Count; i++)
            {
                var span = sourceReplacementSpans[i];
                if (span.Snapshot.TextBuffer == _diskBuffer)
                {
                    insertionSizes[i] += insertionText.Length;
                    break;
                }
            }
        }

        public int GetTypicalInsertionPosition(SnapshotPoint projectionInsertionPoint, ReadOnlyCollection<SnapshotPoint> sourceInsertionPoints)
        {
            return sourceInsertionPoints[0];
        }

        #endregion
    }
}
