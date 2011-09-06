namespace Tvl
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class WeakReference<T> : WeakReference
        where T : class
    {
        public WeakReference(T target)
            : base(target)
        {
        }

        public WeakReference(T target, bool trackResurrection)
            : base(target, trackResurrection)
        {
        }

        protected WeakReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets the object (the target) referenced by the current WeakReference&lt;T&gt; object.
        /// </summary>
        /// <value>
        /// null if the object referenced by the current WeakReference&lt;T&gt; object has been garbage
        /// collected; otherwise, a reference to the object referenced by the current WeakReference&lt;T&gt;
        /// object.
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        /// The reference to the target object is invalid. This exception can be thrown while setting
        /// this property if the value is a null reference or if the object has been finalized during
        /// the set operation.
        /// </exception>
        public virtual new T Target
        {
            get
            {
                return (T)base.Target;
            }
            set
            {
                base.Target = value;
            }
        }
    }
}
