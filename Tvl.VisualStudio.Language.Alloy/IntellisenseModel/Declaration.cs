namespace Tvl.VisualStudio.Language.Alloy.IntellisenseModel
{
    using System;

    internal class Declaration : Element
    {
        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override AlloyFile File
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IElementReference DeclaringElement
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DeclarationAttributes Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsPrivate
        {
            get
            {
                return (Attributes & DeclarationAttributes.Private) != 0;
            }
        }

        public override bool IsExternallyVisible
        {
            get
            {
                if (IsPrivate)
                    return false;

                IElementReference declaringElementReference = DeclaringElement;
                Element declaringElement;
                if (declaringElementReference == null || !declaringElementReference.TryResolve(out declaringElement))
                    return false;

                return declaringElement.IsExternallyVisible;
            }
        }
    }
}
