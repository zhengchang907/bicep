// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Registry;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Collections.Generic;

namespace Bicep.Wasm
{
    public class EmptyModuleRestoreScheduler : IModuleRestoreScheduler
    {
        public void RequestModuleRestore(ICompilationManager compilationManager, DocumentUri documentUri, IEnumerable<ModuleDeclarationSyntax> references, RootConfiguration configuration)
        {
        }

        public void Start()
        {
        }
    }
}
