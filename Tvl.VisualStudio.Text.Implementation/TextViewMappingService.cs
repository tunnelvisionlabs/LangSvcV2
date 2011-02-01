namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITextViewMappingService))]
    [Export(typeof(IWpfTextViewConnectionListener))]
    [ContentType("any")]
    public sealed class TextViewMappingService : ITextViewMappingService, IWpfTextViewConnectionListener
    {
        private static readonly IWpfTextView[] EmptyViews = new IWpfTextView[0];
        private readonly ConcurrentDictionary<WeakReference<ITextBuffer>, List<WeakReference<IWpfTextView>>> _bufferToViewsMap =
            new ConcurrentDictionary<WeakReference<ITextBuffer>, List<WeakReference<IWpfTextView>>>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        #region ITextViewMappingService Members

        public IEnumerable<IWpfTextView> GetViewsForBuffer(ITextBuffer buffer)
        {
            WeakReference<ITextBuffer> weakBuffer = new WeakReference<ITextBuffer>(buffer);

            List<WeakReference<IWpfTextView>> views;
            if (!_bufferToViewsMap.TryGetValue(weakBuffer, out views))
                return EmptyViews;

            lock (views)
            {
                return views.Select(reference => reference.Target).Where(target => target != null).ToArray();
            }
        }

        #endregion

        #region IWpfTextViewConnectionListener Members

        public void SubjectBuffersConnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            WeakReference<IWpfTextView> weakView = new WeakReference<IWpfTextView>(textView);
            foreach (var buffer in subjectBuffers)
            {
                var key = new WeakReference<ITextBuffer>(buffer);
                List<WeakReference<IWpfTextView>> views = _bufferToViewsMap.GetOrAdd(key, CreateViewList);
                lock (views)
                {
                    if (!views.Contains(weakView))
                        views.Add(weakView);
                }
            }
        }

        public void SubjectBuffersDisconnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            WeakReference<IWpfTextView> weakView = new WeakReference<IWpfTextView>(textView);
            foreach (var buffer in subjectBuffers)
            {
                List<WeakReference<IWpfTextView>> views;
                var key = new WeakReference<ITextBuffer>(buffer);
                if (_bufferToViewsMap.TryGetValue(key, out views))
                {
                    lock (views)
                    {
                        views.Remove(weakView);
                        RemoveDeadReferences(views);
                    }
                }
            }

            RemoveDeadReferences(_bufferToViewsMap);
        }

        private static void RemoveDeadReferences(ConcurrentDictionary<WeakReference<ITextBuffer>, List<WeakReference<IWpfTextView>>> dictionary)
        {
            foreach (var key in dictionary.Keys.ToArray())
            {
                List<WeakReference<IWpfTextView>> value;
                if (!key.IsAlive)
                    dictionary.TryRemove(key, out value);
            }
        }

        private static void RemoveDeadReferences(List<WeakReference<IWpfTextView>> list)
        {
            list.RemoveAll(reference => !reference.IsAlive);
        }

        private static List<WeakReference<IWpfTextView>> CreateViewList(WeakReference<ITextBuffer> buffer)
        {
            return new List<WeakReference<IWpfTextView>>();
        }

        #endregion

        private class WeakReference<T> : WeakReference
            where T : class
        {
            private int _hashCode;

            public WeakReference(T target)
                : base(target)
            {
                Contract.Requires(target != null);
                this._hashCode = RuntimeHelpers.GetHashCode(target);
            }

            public new T Target
            {
                get
                {
                    return (T)base.Target;
                }
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(obj, this))
                    return true;
                if (obj == null)
                    return false;

                WeakReference<T> weakOther = obj as WeakReference<T>;
                if (weakOther != null)
                    return this.GetHashCode() == weakOther.GetHashCode();

                return object.ReferenceEquals(this.Target, obj);
            }

            public override int GetHashCode()
            {
                return this._hashCode;
            }
        }
    }
}
