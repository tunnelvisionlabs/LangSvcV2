#if DEBUG

// Field 'field name' is never assigned to, and will always have its default value null
#pragma warning disable 649

namespace Tvl.VisualStudio.Shell.Commands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows.Input;
    using Microsoft.VisualStudio.Utilities;
    using DefaultValueAttribute = System.ComponentModel.DefaultValueAttribute;

    internal static class ProvidedCommandsTest
    {
        [Export]
        [Name("Tvl.Test.TestButton")]
        [DisplayName("Test Button")]
        [ExportMetadata("CommandFlag", "ButtonFlags...")]
        [ExportMetadata("ButtonText", "Button Text")]
        [ExportMetadata("CommandName", "Command Name")]
        [ExportMetadata("LocalizedCanonicalName", "Localized canonical name")]
        [ExportMetadata("MenuText", "Menu text")]
        [ExportMetadata("ToolTip", "Tool tip")]
        [Priority(100)]
        [ButtonCommandOptions(ButtonCommandOptions.AllowParameters | ButtonCommandOptions.DefaultDisabled)]
        public static ButtonDefinition TestButton;
    }

    // Required Metadata: Name, DisplayName
    // Optional Metadata: CommandPlacement, KeyBinding, Parent, Priority, ToolbarPriority, ButtonCommandOptions
    public sealed class ButtonDefinition
    {
    }

    public sealed class MenuDefinition
    {
    }

    public sealed class ComboDefinition
    {
    }

    // Required Metadata: Name
    // Optional Metadata: Priority, ToolbarPriority, CommandGroupOptions
    public sealed class CommandGroupDefinition
    {
    }

    public interface ICommandDefinitionMetadata
    {
        string Name
        {
            get;
        }

        string DisplayName
        {
            get;
        }

        IEnumerable<CommandPlacement> CommandPlacement
        {
            get;
        }

        IEnumerable<KeyBinding> KeyBinding
        {
            get;
        }

        [DefaultValue("")]
        string Parent
        {
            get;
        }

        [DefaultValue(100)]
        int Priority
        {
            get;
        }

        [DefaultValue(100)]
        int ToolbarPriority
        {
            get;
        }
    }

    public interface IButtonDefinitionMetadata : ICommandDefinitionMetadata
    {
        [DefaultValue(ButtonCommandOptions.None)]
        ButtonCommandOptions Options
        {
            get;
        }

        [DefaultValue(ButtonType.Button)]
        ButtonType Type
        {
            get;
        }
    }

    [Flags]
    public enum MenuCommandOptions
    {
        None = 0,
        AlwaysCreate = 1 << 0,
        DefaultDocked = 1 << 1,
        DefaultInvisible = 1 << 2,
        NoCache = 1 << 3,
        DynamicVisibility = 1 << 4,
        IconAndText = 1 << 5,
        NoCustomize = 1 << 6,
        NotInToolbarList = 1 << 7,
        NoToolbarClose = 1 << 8,
        TextChanges = 1 << 9,
        TextIsAnchorCommand = 1 << 10
    }

    [Flags]
    public enum CommandGroupOptions
    {
        None = 0,
        Dynamic = 1 << 0
    }

    [Flags]
    public enum ComboCommandOptions
    {
        None = 0,
        CaseSensitive = 1 << 0,
        CommandWellOnly = 1 << 1,
        DefaultDisabled = 1 << 2,
        DefaultInvisible = 1 << 3,
        DynamicVisibility = 1 << 4,
        FilterKeys = 1 << 5,
        IconAndText = 1 << 6,
        NoAutoComplete = 1 << 7,
        NoButtonCustomize = 1 << 8,
        NoCustomize = 1 << 9,
        NoKeyCustomize = 1 << 10,
        StretchHorizontally = 1 << 11
    }

    public enum MenuType
    {
        Menu,
        MenuController,
        MenuControllerLatched,
        Toolbar,
        ToolWindowToolbar,
        Context,
    }

    public enum ComboType
    {
        DropDown,
        Dynamic,
        Index,
        MRU,
        AppID,
        Shared
    }

    public enum ButtonType
    {
        Button,
        MenuButton,
        SplitDropDown,
        Swatch,
        AppID,
        Shared
    }

    [Flags]
    public enum ButtonCommandOptions
    {
        None = 0,
        AllowParameters = 1 << 0,
        CommandWellOnly = 1 << 1,
        DefaultDisabled = 1 << 2,
        DefaultInvisible = 1 << 3,
        NoCache = 1 << 4,
        DynamicItemStart = 1 << 5,
        DynamicVisibility = 1 << 6,
        FixMenuController = 1 << 7,
        IconAndText = 1 << 8,
        NoButtonCustomize = 1 << 9,
        NoCustomize = 1 << 10,
        NoKeyCustomize = 1 << 11,
        NoShowOnMenuController = 1 << 12,
        Pict = 1 << 13,
        PostExec = 1 << 14,
        ProfferedCommand = 1 << 15,
        RouteToDocs = 1 << 16,
        TextCascadeUseButton = 1 << 17,
        TextChanges = 1 << 18,
        TextChangesButton = 1 << 19,
        TextContextUseButton = 1 << 20,
        TextMenuControlUseButton = 1 << 21,
        TextMenuUseButton = 1 << 22,
        TextOnly = 1 << 23
    }

    public sealed class ParentAttribute : SingletonBaseMetadataAttribute
    {
        public ParentAttribute(string parent)
        {
            this.Parent = parent;
        }

        public string Parent
        {
            get;
            private set;
        }
    }

    public sealed class PriorityAttribute : SingletonBaseMetadataAttribute
    {
        public PriorityAttribute(int priority)
        {
            this.Priority = priority;
        }

        public int Priority
        {
            get;
            private set;
        }
    }

    public sealed class ToolbarPriorityAttribute : SingletonBaseMetadataAttribute
    {
        public ToolbarPriorityAttribute(int priority)
        {
            this.ToolbarPriority = priority;
        }

        public int ToolbarPriority
        {
            get;
            private set;
        }
    }

    public sealed class ButtonCommandOptionsAttribute : SingletonBaseMetadataAttribute
    {
        public ButtonCommandOptionsAttribute(ButtonCommandOptions options)
        {
            this.ButtonCommandOptions = options;
        }

        public ButtonCommandOptions ButtonCommandOptions
        {
            get;
            private set;
        }
    }

    public sealed class MenuCommandOptionsAttribute : SingletonBaseMetadataAttribute
    {
        public MenuCommandOptionsAttribute(MenuCommandOptions options)
        {
            this.MenuCommandOptions = options;
        }

        public MenuCommandOptions MenuCommandOptions
        {
            get;
            private set;
        }
    }

    public sealed class ComboCommandOptionsAttribute : SingletonBaseMetadataAttribute
    {
        public ComboCommandOptionsAttribute(ComboCommandOptions options)
        {
            this.ComboCommandOptions = options;
        }

        public ComboCommandOptions ComboCommandOptions
        {
            get;
            private set;
        }
    }

    public sealed class CommandGroupOptionsAttribute : SingletonBaseMetadataAttribute
    {
        public CommandGroupOptionsAttribute(CommandGroupOptions options)
        {
            this.CommandGroupOptions = options;
        }

        public CommandGroupOptions CommandGroupOptions
        {
            get;
            private set;
        }
    }

    public class KeyBinding
    {
        public KeyBinding(string context, Key key, ModifierKeys modifiers)
            : this(context, key, Key.None, modifiers)
        {
        }

        public KeyBinding(string context, Key firstKey, Key secondKey, ModifierKeys modifiers)
        {
            this.FirstKey = firstKey;
            this.SecondKey = secondKey;
            this.Modifiers = modifiers;
        }

        public string Context
        {
            get;
            private set;
        }

        public Key FirstKey
        {
            get;
            private set;
        }

        public Key SecondKey
        {
            get;
            private set;
        }

        public ModifierKeys Modifiers
        {
            get;
            private set;
        }
    }

    public sealed class KeyBindingAttribute : MultipleBaseMetadataAttribute
    {
        public KeyBindingAttribute(string context, Key key, ModifierKeys modifiers)
            : this(context, key, Key.None, modifiers)
        {
        }

        public KeyBindingAttribute(string context, Key firstKey, Key secondKey, ModifierKeys modifiers)
        {
            this.KeyBinding = new KeyBinding(context, firstKey, secondKey, modifiers);
        }

        public KeyBinding KeyBinding
        {
            get;
            private set;
        }
    }

    public class CommandPlacement
    {
        public CommandPlacement(string parent, int priority)
        {
            this.Parent = parent;
            this.Priority = priority;
        }

        public string Parent
        {
            get;
            private set;
        }

        public int Priority
        {
            get;
            private set;
        }
    }

    public sealed class CommandPlacementAttribute : MultipleBaseMetadataAttribute
    {
        public CommandPlacementAttribute(string parent)
        {
            this.Parent = parent;
        }

        public string Parent
        {
            get;
            private set;
        }

        public int Priority
        {
            get;
            set;
        }

        public CommandPlacement CommandPlacement
        {
            get
            {
                return new CommandPlacement(this.Parent, this.Priority);
            }
        }
    }

    //[Export(typeof(ICommandHandler))]
    //internal class TestCommandProvider : ICommandHandler
    //{
    //    void foo()
    //    {
    //        OleMenuCommand command;
    //    }
    //}

    //internal class Command : OleMenuCommand
    //{
    //    override 
    //}
}

#endif
