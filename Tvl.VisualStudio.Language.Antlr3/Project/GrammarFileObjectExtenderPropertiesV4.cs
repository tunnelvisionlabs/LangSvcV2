namespace Tvl.VisualStudio.Language.Antlr3.Project
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    [ComVisible(true)]
    public class GrammarFileObjectExtenderPropertiesV4
    {
        private readonly IVsBuildPropertyStorage _buildPropertyStorage;
        private readonly uint _itemId;

        public GrammarFileObjectExtenderPropertiesV4(IVsBuildPropertyStorage buildPropertyStorage, uint itemId)
        {
            Contract.Requires<ArgumentNullException>(buildPropertyStorage != null, "buildPropertyStorage");

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
        [DefaultValue(true)]
        [DisplayName("Generate Listener")]
        [Description("Generates a parse tree listener for the grammar.")]
        public bool GenerateListener
        {
            get
            {
                string value;
                if (ErrorHandler.Failed(_buildPropertyStorage.GetItemAttribute(_itemId, "Listener", out value)))
                    return false;

                return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }

            set
            {
                ErrorHandler.ThrowOnFailure(_buildPropertyStorage.SetItemAttribute(_itemId, "Listener", value.ToString()));
            }
        }

        [Category("ANTLR")]
        [DefaultValue(true)]
        [DisplayName("Generate Visitor")]
        [Description("Generates a parse tree visitor for the grammar.")]
        public bool GenerateVisitor
        {
            get
            {
                string value;
                if (ErrorHandler.Failed(_buildPropertyStorage.GetItemAttribute(_itemId, "Visitor", out value)))
                    return false;

                return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }

            set
            {
                ErrorHandler.ThrowOnFailure(_buildPropertyStorage.SetItemAttribute(_itemId, "Visitor", value.ToString()));
            }
        }

        [Category("ANTLR")]
        [DefaultValue(false)]
        [DisplayName("Force ATN")]
        [Description("Forces the parser to use AdaptivePredict for all decisions, including LL(1) decisions.")]
        public bool ForceAtn
        {
            get
            {
                string value;
                if (ErrorHandler.Failed(_buildPropertyStorage.GetItemAttribute(_itemId, "ForceAtn", out value)))
                    return false;

                return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }

            set
            {
                ErrorHandler.ThrowOnFailure(_buildPropertyStorage.SetItemAttribute(_itemId, "ForceAtn", value.ToString()));
            }
        }

        [Category("ANTLR")]
        [DefaultValue(false)]
        [DisplayName("Abstract Grammar")]
        [Description("When true, the generated classes are marked as abstract.")]
        public bool Profile
        {
            get
            {
                string value;
                if (ErrorHandler.Failed(_buildPropertyStorage.GetItemAttribute(_itemId, "Abstract", out value)))
                    return false;

                return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }

            set
            {
                ErrorHandler.ThrowOnFailure(_buildPropertyStorage.SetItemAttribute(_itemId, "Abstract", value.ToString()));
            }
        }
    }
}
