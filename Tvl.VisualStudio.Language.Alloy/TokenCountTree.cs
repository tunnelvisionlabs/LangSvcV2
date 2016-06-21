namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using Microsoft.VisualStudio.Text;

    public class TokenCountTree
    {
        private readonly TokenCounter _counter;
        private readonly ITextSnapshot _currentSnapshot;

        public TokenCountTree(TokenCounter counter, ITextSnapshot currentSnapshot)
        {
            if (counter == null)
                throw new ArgumentNullException("counter");
            if (currentSnapshot == null)
                throw new ArgumentNullException("currentSnapshot");

            _counter = counter;
            _currentSnapshot = currentSnapshot;
        }


    }
}
