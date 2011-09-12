namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Collections.ObjectModel;
    using Tvl.Collections;
    using System.Diagnostics.Contracts;

    public partial class JavaApplicationPropertyPagePanel : JavaPropertyPagePanel
    {
        private static readonly string DisplayJavaArchiveOutputType = "Java Archive (jar)";
        private static readonly string DisplayNotSetStartupObject = "(Not Set)";

        private static readonly ImmutableList<string> _emptyList;

        private ImmutableList<string> _availableTargetVirtualMachines = _emptyList;
        private ImmutableList<string> _availableOutputTypes = _emptyList;
        private ImmutableList<string> _availableStartupObjects = _emptyList;

        public JavaApplicationPropertyPagePanel()
            : this(null)
        {
        }

        public JavaApplicationPropertyPagePanel(JavaApplicationPropertyPage parentPropertyPage)
            : base(parentPropertyPage)
        {
            InitializeComponent();
        }

        internal new JavaApplicationPropertyPage ParentPropertyPage
        {
            get
            {
                return (JavaApplicationPropertyPage)base.ParentPropertyPage;
            }
        }

        public ImmutableList<string> AvailableTargetVirtualMachines
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableList<string>>() != null);

                return _availableTargetVirtualMachines;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");
                Contract.Requires<ArgumentException>(Contract.ForAll(value, i => string.IsNullOrEmpty(i)));

                if (_availableTargetVirtualMachines.SequenceEqual(value, StringComparer.CurrentCulture))
                    return;

                _availableTargetVirtualMachines = value;
                cmbTargetVirtualMachine.Items.Clear();
                cmbTargetVirtualMachine.Items.AddRange(value.ToArray());
            }
        }

        public ImmutableList<string> AvailableOutputTypes
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableList<string>>() != null);

                return _availableOutputTypes;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");
                Contract.Requires<ArgumentException>(Contract.ForAll(value, i => string.IsNullOrEmpty(i)));

                if (_availableOutputTypes.SequenceEqual(value, StringComparer.CurrentCulture))
                    return;

                _availableOutputTypes = value;
                cmbOutputType.Items.Clear();
                cmbOutputType.Items.AddRange(value.ToArray());
            }
        }

        public ImmutableList<string> AvailableStartupObjects
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableList<string>>() != null);

                return _availableStartupObjects;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");
                Contract.Requires<ArgumentException>(Contract.ForAll(value, i => string.IsNullOrEmpty(i)));

                if (_availableStartupObjects.SequenceEqual(value, StringComparer.CurrentCulture))
                    return;

                _availableStartupObjects = value;
                cmbStartupObject.Items.Clear();
                cmbStartupObject.Items.AddRange(value.ToArray());
            }
        }

        public string PackageName
        {
            get
            {
                return txtPackageName.Text;
            }

            set
            {
                txtPackageName.Text = value ?? string.Empty;
            }
        }

        public string TargetVirtualMachine
        {
            get
            {
                if (cmbTargetVirtualMachine.SelectedItem == null)
                    return string.Empty;

                return cmbTargetVirtualMachine.SelectedItem.ToString();
            }

            set
            {
                cmbTargetVirtualMachine.SelectedItem = value;
            }
        }

        public string OutputType
        {
            get
            {
                if (cmbOutputType.SelectedItem == null)
                    return string.Empty;

                return cmbOutputType.SelectedItem.ToString();
            }

            set
            {
                cmbOutputType.SelectedItem = value;
            }
        }

        public string StartupObject
        {
            get
            {
                if (cmbStartupObject.SelectedItem == null)
                    return string.Empty;

                return cmbStartupObject.SelectedItem.ToString();
            }

            set
            {
                cmbStartupObject.SelectedItem = value;
            }
        }
    }
}
