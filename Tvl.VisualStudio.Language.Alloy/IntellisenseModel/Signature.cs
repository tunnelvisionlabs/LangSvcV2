namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Language.Intellisense;
    using ImageSource = System.Windows.Media.ImageSource;

    internal class Signature : Element
    {
        private readonly string _name;
        private readonly AlloyFile _file;
        private readonly SignatureAttributes _attributes;

        public Signature(string name, AlloyFile file, SignatureAttributes attributes)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (file == null)
                throw new ArgumentNullException("file");

            _name = name;
            _file = file;
            _attributes = attributes;
        }

        public override string Name
        {
            get
            {
                return _name;
            }
        }

        public override AlloyFile File
        {
            get
            {
                return _file;
            }
        }

        public override bool IsExternallyVisible
        {
            get
            {
                return !IsPrivate;
            }
        }

        public SignatureAttributes Attributes
        {
            get
            {
                return _attributes;
            }
        }

        public bool IsAbstract
        {
            get
            {
                return (Attributes & SignatureAttributes.Abstract) != 0;
            }
        }

        public bool IsPrivate
        {
            get
            {
                return (Attributes & SignatureAttributes.Private) != 0;
            }
        }

        public bool IsEnum
        {
            get
            {
                return (Attributes & SignatureAttributes.Enum) != 0;
            }
        }

        public virtual string DeclarationText
        {
            get
            {
                if (IsEnum && IsAbstract)
                {
                    return string.Format("enum {0}", Name);
                }

                List<string> modifiers = new List<string>();
                if (IsAbstract)
                    modifiers.Add("abstract");
                if (IsPrivate)
                    modifiers.Add("private");
                switch (Attributes & SignatureAttributes.MultiplicityMask)
                {
                case SignatureAttributes.One:
                    modifiers.Add("one");
                    break;

                case SignatureAttributes.OneOrMore:
                    modifiers.Add("some");
                    break;

                case SignatureAttributes.ZeroOrOne:
                    modifiers.Add("lone");
                    break;

                default:
                    break;
                }

                string modifierString = modifiers.Count > 0 ? string.Join(" ", modifiers) + ' ' : string.Empty;
                return string.Format("{0}sig {1}", modifierString, Name);
            }
        }

        public IEnumerable<Declaration> Declarations
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IElementReference<Signature> Extends
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IElementReference<Signature>> Within
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Completion CreateCompletion(AlloyIntellisenseController controller, ICompletionSession session)
        {
            string displayText = Name;
            string insertionText = Name;
            string description = string.Empty;
            StandardGlyphGroup glyphGroup = IsEnum ? StandardGlyphGroup.GlyphGroupEnum : StandardGlyphGroup.GlyphGroupStruct;
            ImageSource iconSource = controller.Provider.GlyphService.GetGlyph(glyphGroup, StandardGlyphItem.GlyphItemPublic);
            string iconAutomationText = string.Empty;
            return new Completion(displayText, insertionText, description, iconSource, iconAutomationText);
        }

        public override void GetQuickInfo(AlloyIntellisenseController controller, IQuickInfoSession session, IList<object> content)
        {
            content.Add(DeclarationText);
        }
    }
}
