/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * This source code has been modified from its original form.
 *
 * ***************************************************************************/

namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.Runtime.InteropServices;

    [Guid("8757E177-F1D0-4955-8AE5-48AE6DBD6237")]
    public class PhpEditorFactoryWithEncoding : PhpEditorFactory
    {
        public PhpEditorFactoryWithEncoding(PhpLanguagePackage package)
            : base(package, true)
        {
        }
    }
}
