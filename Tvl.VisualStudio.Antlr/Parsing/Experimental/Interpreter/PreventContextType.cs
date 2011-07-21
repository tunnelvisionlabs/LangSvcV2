namespace Tvl.VisualStudio.Language.Parsing.Experimental.Interpreter
{
    public enum PreventContextType
    {
        /// <summary>
        /// Don't block any transitions.
        /// </summary>
        None,

        /// <summary>
        /// Block all push transitions.
        /// </summary>
        Push,

        ///// <summary>
        ///// Block all non-recursive push transitions.
        ///// </summary>
        //PushNonRecursive,

        /// <summary>
        /// Block all pop transitions.
        /// </summary>
        Pop,

        ///// <summary>
        ///// Block all non-recursive pop transitions.
        ///// </summary>
        //PopNonRecursive,
    }
}
