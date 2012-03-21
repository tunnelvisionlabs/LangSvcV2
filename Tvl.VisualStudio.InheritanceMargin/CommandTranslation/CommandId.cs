namespace Tvl.VisualStudio.InheritanceMargin.CommandTranslation
{
    using System.Collections.Generic;

    using Guid = System.Guid;

    public sealed class CommandId
    {
        public static readonly IEqualityComparer<CommandId> DictionaryEqualityComparer = new EqualityComparer();

        public CommandId(Guid menuGroup, int commandID)
            : this(menuGroup, commandID, commandID)
        {
        }

        public CommandId(Guid menuGroup, int startID, int endID)
        {
            this.Guid = menuGroup;
            this.Id = startID;
            this.EndId = endID;
        }

        public int EndId
        {
            get;
            private set;
        }

        public Guid Guid
        {
            get;
            private set;
        }

        public int Id
        {
            get;
            private set;
        }

        private class EqualityComparer : IEqualityComparer<CommandId>
        {
            public bool Equals(CommandId x, CommandId y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x != null)
                {
                    if (y == null)
                        return false;

                    if (x.Guid == y.Guid)
                    {
                        if ((y.Id <= x.Id) && (y.EndId >= x.Id))
                            return true;

                        if ((x.Id <= y.Id) && (x.EndId >= y.Id))
                            return true;
                    }
                }

                return false;
            }

            public int GetHashCode(CommandId obj)
            {
                if (obj != null)
                    return obj.Guid.GetHashCode();

                return 0;
            }
        }
    }
}
