namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class SingleFileGeneratorNodeExtenderProperties : LocalizableProperties
    {
        private readonly HierarchyNode _node;

        public SingleFileGeneratorNodeExtenderProperties(HierarchyNode node)
            : base(node.ProjectManager)
        {
            Contract.Requires<ArgumentNullException>(node != null, "node");
            _node = node;
        }

        internal event EventHandler<HierarchyNodeEventArgs> CustomToolChanged;
        internal event EventHandler<HierarchyNodeEventArgs> CustomToolNamespaceChanged;

        [Browsable(false)]
        [AutomationBrowsable(false)]
        public HierarchyNode Node
        {
            get
            {
                return _node;
            }
        }

        [SRCategoryAttribute(SR.Advanced)]
        [LocDisplayName(SR.CustomTool)]
        [SRDescriptionAttribute(SR.CustomToolDescription)]
        public virtual string CustomTool
        {
            get
            {
                return this.Node.ItemNode.GetMetadata(ProjectFileConstants.Generator);
            }

            set
            {
                if (CustomTool != value)
                {
                    this.Node.ItemNode.SetMetadata(ProjectFileConstants.Generator, value != string.Empty ? value : null);
                    HierarchyNodeEventArgs args = new HierarchyNodeEventArgs(this.Node);
                    OnCustomToolChanged(args);
                }
            }
        }

        [SRCategoryAttribute(VisualStudio.Project.SR.Advanced)]
        [LocDisplayName(SR.CustomToolNamespace)]
        [SRDescriptionAttribute(SR.CustomToolNamespaceDescription)]
        public virtual string CustomToolNamespace
        {
            get
            {
                return this.Node.ItemNode.GetMetadata(ProjectFileConstants.CustomToolNamespace);
            }

            set
            {
                if (CustomToolNamespace != value)
                {
                    this.Node.ItemNode.SetMetadata(ProjectFileConstants.CustomToolNamespace, value != String.Empty ? value : null);
                    HierarchyNodeEventArgs args = new HierarchyNodeEventArgs(this.Node);
                    OnCustomToolNamespaceChanged(args);
                }
            }
        }

        protected virtual void OnCustomToolChanged(HierarchyNodeEventArgs e)
        {
            var t = CustomToolChanged;
            if (t != null)
                t(this, e);
        }

        protected virtual void OnCustomToolNamespaceChanged(HierarchyNodeEventArgs e)
        {
            var t = CustomToolNamespaceChanged;
            if (t != null)
                t(this, e);
        }
    }
}
