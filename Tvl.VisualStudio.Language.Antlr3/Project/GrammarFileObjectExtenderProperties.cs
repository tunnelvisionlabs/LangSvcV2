namespace Tvl.VisualStudio.Language.Antlr3.Project
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    [ComVisible(true)]
    public class GrammarFileObjectExtenderProperties
    {
        private readonly IVsBuildPropertyStorage _buildPropertyStorage;
        private readonly uint _itemId;

        public GrammarFileObjectExtenderProperties([NotNull] IVsBuildPropertyStorage buildPropertyStorage, uint itemId)
        {
            Requires.NotNull(buildPropertyStorage, nameof(buildPropertyStorage));

            _buildPropertyStorage = buildPropertyStorage;
            _itemId = itemId;
        }

        [Category("ANTLR")]
        [DefaultValue("(Not Set)")]
        [DisplayName("Language Target")]
        [Description("Explicitly specifies the grammar's target language. If set, this property overrides any value specified within the grammar itself.")]
        public string LanguageTarget
        {
            get
            {
                string value;
                if (ErrorHandler.Failed(_buildPropertyStorage.GetItemAttribute(_itemId, "TargetLanguage", out value)) || string.IsNullOrWhiteSpace(value))
                    return "(Not Set)";

                return value;
            }

            set
            {
                ErrorHandler.ThrowOnFailure(_buildPropertyStorage.SetItemAttribute(_itemId, "TargetLanguage", value));
            }
        }

        [Category("ANTLR")]
        [DefaultValue(false)]
        [DisplayName("Debug Grammar")]
        [Description("Builds a debug version of the grammar. Note that this feature may not be available for all language targets.")]
        public bool DebugGrammar
        {
            get
            {
                string value;
                if (ErrorHandler.Failed(_buildPropertyStorage.GetItemAttribute(_itemId, "DebugGrammar", out value)))
                    return false;

                return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }

            set
            {
                ErrorHandler.ThrowOnFailure(_buildPropertyStorage.SetItemAttribute(_itemId, "DebugGrammar", value.ToString()));
            }
        }

        [Category("ANTLR")]
        [DefaultValue(false)]
        [DisplayName("Profile Grammar")]
        [Description("Builds a version of the grammar for profiling. Note that this feature may not be available for all language targets.")]
        public bool ProfileGrammar
        {
            get
            {
                string value;
                if (ErrorHandler.Failed(_buildPropertyStorage.GetItemAttribute(_itemId, "ProfileGrammar", out value)))
                    return false;

                return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }

            set
            {
                ErrorHandler.ThrowOnFailure(_buildPropertyStorage.SetItemAttribute(_itemId, "ProfileGrammar", value.ToString()));
            }
        }
    }
}
