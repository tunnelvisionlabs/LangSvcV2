namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.Events;
    using Tvl.Extensions;
    using Tvl.VisualStudio.Language.Alloy.IntellisenseModel;
    using ReaderWriterLockSlim = System.Threading.ReaderWriterLockSlim;

    [Export]
    internal sealed partial class AlloyIntellisenseCache : IPartImportsSatisfiedNotification
    {
        private readonly ReaderWriterLockSlim _updateLock = new ReaderWriterLockSlim();

        private readonly Dictionary<AlloyFileReference, AlloyFile> _files = new Dictionary<AlloyFileReference, AlloyFile>(UniqueFileComparer.Default);

        public bool TryResolveContext(AlloyPositionReference position, out Element element)
        {
            element = null;
            return false;
        }

#if TRACK_CONTENT_TYPES
        private readonly List<WeakReference<ITextBuffer>> _textBuffers = new List<WeakReference<ITextBuffer>>();

        [Import]
        private ITextBufferFactoryService TextBufferFactoryService
        {
            get;
            set;
        }

        [Import]
        private IContentTypeRegistryService ContentTypeRegistryService
        {
            get;
            set;
        }
#endif

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
#if TRACK_CONTENT_TYPES
            TextBufferFactoryService.TextBufferCreated += WeakEvents.AsWeak<TextBufferCreatedEventArgs>(HandleTextBufferCreated, handler => TextBufferFactoryService.TextBufferCreated -= handler);
#endif
        }

#if TRACK_CONTENT_TYPES
        private void HandleTextBufferCreated(object sender, TextBufferCreatedEventArgs e)
        {
            using (var writeLock = _updateLock.WriteLock())
            {
                ITextBuffer buffer = e.TextBuffer;
                if (buffer != null)
                {
                    buffer.ContentTypeChanged += WeakEvents.AsWeak<ContentTypeChangedEventArgs>(HandleBufferContentTypeChanged, handler => buffer.ContentTypeChanged -= handler);
                    _textBuffers.Add(new WeakReference<ITextBuffer>(e.TextBuffer));
                }
            }
        }

        private void HandleBufferContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
        }
#endif
    }
}
