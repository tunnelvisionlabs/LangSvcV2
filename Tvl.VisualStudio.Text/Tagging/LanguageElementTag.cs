namespace Tvl.VisualStudio.Text.Tagging
{
    using System.Windows.Media;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;

    public class LanguageElementTag : ILanguageElementTag
    {
        public LanguageElementTag([NotNull] string name, [NotNull] string category, ImageSource glyph, SnapshotSpan target)
        {
            Requires.NotNullOrEmpty(name, nameof(name));
            Requires.NotNullOrEmpty(category, nameof(category));

            this.Name = name;
            this.Category = category;
            this.Glyph = glyph;
            this.Target = target;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Category
        {
            get;
            private set;
        }

        public ImageSource Glyph
        {
            get;
            private set;
        }

        public SnapshotSpan Target
        {
            get;
            private set;
        }
    }
}
