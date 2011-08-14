//--------------------------------------------------------------------------------------------------
// <copyright file="IProjectSourceNode.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
//    
//    The use and distribution terms for this software are covered by the
//    Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
//    which can be found in the file CPL.TXT at the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by
//    the terms of this license.
//    
//    You must not remove this notice, or any other, from this software.
// </copyright>
// 
// <summary>
// Contains the IProjectSourceNode interface.
// </summary>
//--------------------------------------------------------------------------------------------------

namespace Microsoft.VisualStudio.Project
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This interface provides the ability to identify the items which have the cability of including / excluding
    /// themselves to / from the project system. It also tells if the item is a member of the project or not.
    /// </summary>
    public interface IProjectSourceNode
    {
        /// <summary>
        /// Gets if the item is not a member of the project.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "NonMember")]
        bool IsNonMemberItem
        {
            get;
        }

        /// <summary>
        /// Exclude the item from the project system.
        /// </summary>
        /// <returns>Returns success or failure code.</returns>
        int ExcludeFromProject();

        /// <summary>
        /// Include the item into the project system.
        /// </summary>
        /// <returns>Returns success or failure code.</returns>
        int IncludeInProject();

        /// <summary>
        /// Include the item into the project system recursively.
        /// </summary>
        /// <param name="recursive">Flag that indicates if the inclusion should be recursive or not.</param>
        /// <returns>Returns success or failure code.</returns>
        int IncludeInProject(bool recursive);
    }
}
