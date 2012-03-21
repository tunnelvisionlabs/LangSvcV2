namespace Tvl.VisualStudio.InheritanceMargin.CommandTranslation
{
    public class CommandTargetParameters
    {
        private CommandTargetParameters(string text, int id)
        {
            this.Text = text;
            this.Id = id;
        }

        public bool Enabled
        {
            get;
            set;
        }

        public int Id
        {
            get;
            private set;
        }

        public object InArgs
        {
            get;
            set;
        }

        public bool Pressed
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public bool Visible
        {
            get;
            set;
        }

        public static CommandTargetParameters CreateInstance(uint id)
        {
            return new CommandTargetParameters(string.Empty, (int)id);
        }

        public static CommandTargetParameters CreateInstance(uint id, string text)
        {
            return new CommandTargetParameters(text, (int)id);
        }
    }
}
