namespace Tvl.VisualStudio.Text.Navigation
{
    using System.Diagnostics.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [ContractClassFor(typeof(IEditorNavigationTypeRegistryService))]
    public abstract class IEditorNavigationTypeRegistryServiceContracts : IEditorNavigationTypeRegistryService
    {
        IEditorNavigationType IEditorNavigationTypeRegistryService.CreateEditorNavigationType(EditorNavigationTypeDefinition definition, string type, IEnumerable<IEditorNavigationType> baseTypes)
        {
            Contract.Requires<ArgumentNullException>(definition != null);
            Contract.Requires<ArgumentNullException>(type != null);
            Contract.Ensures(Contract.Result<IEditorNavigationType>() != null);

            throw new NotImplementedException();
        }

        IEditorNavigationType IEditorNavigationTypeRegistryService.CreateTransientEditorNavigationType(IEnumerable<IEditorNavigationType> baseTypes)
        {
            Contract.Requires<ArgumentNullException>(baseTypes != null);
            Contract.Requires<ArgumentException>(baseTypes.Any());
            Contract.Ensures(Contract.Result<IEditorNavigationType>() != null);

            throw new NotImplementedException();
        }

        IEditorNavigationType IEditorNavigationTypeRegistryService.CreateTransientEditorNavigationType(params IEditorNavigationType[] baseTypes)
        {
            Contract.Requires<ArgumentNullException>(baseTypes != null);
            Contract.Requires<ArgumentException>(baseTypes.Length > 0);
            Contract.Ensures(Contract.Result<IEditorNavigationType>() != null);

            throw new NotImplementedException();
        }

        IEditorNavigationType IEditorNavigationTypeRegistryService.GetEditorNavigationType(string type)
        {
            Contract.Requires<ArgumentNullException>(type != null);

            throw new NotImplementedException();
        }
    }
}
